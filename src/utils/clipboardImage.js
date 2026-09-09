export function getClipboardImages(event) {
  const data = event.clipboardData
  if (!data) return []
  const files = Array.from(data.items || [])
    .filter((item) => item.kind === 'file')
    .map((item) => item.getAsFile())
    .filter(Boolean)
  return (files.length ? files : Array.from(data.files || []))
    .filter((file) => file.type.startsWith('image/'))
}

// Quill's image chooser inserts data URLs; use the same representation on paste.
export function enableQuillImagePaste(quill, onError) {
  let disposed = false
  const paste = async (event) => {
    if (!quill.isEnabled() || event.defaultPrevented) return
    const files = getClipboardImages(event)
    if (!files.length) return
    event.preventDefault()
    event.stopImmediatePropagation()
    const selection = quill.getSelection() || { index: quill.getLength() - 1, length: 0 }
    try {
      const images = await Promise.all(files.map((file) => new Promise((resolve, reject) => {
        const reader = new FileReader()
        reader.onload = () => resolve(reader.result)
        reader.onerror = () => reject(reader.error)
        reader.readAsDataURL(file)
      })))
      if (disposed || !quill.isEnabled() || !quill.root.isConnected) return
      const index = Math.min(selection.index, quill.getLength() - 1)
      const length = Math.min(selection.length, quill.getLength() - 1 - index)
      const ops = []
      if (index) ops.push({ retain: index })
      if (length) ops.push({ delete: length })
      images.forEach((image) => ops.push({ insert: { image } }))
      quill.updateContents({ ops }, 'user')
      quill.setSelection(index + images.length, 0, 'silent')
    } catch (error) {
      if (!disposed) onError?.(error)
    }
  }
  quill.root.addEventListener('paste', paste, true)
  return () => {
    disposed = true
    quill.root.removeEventListener('paste', paste, true)
  }
}
