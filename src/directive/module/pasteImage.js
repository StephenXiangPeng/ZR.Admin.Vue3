import { getClipboardImages } from '../../utils/clipboardImage'
import './pasteImage.css'

const states = new WeakMap()

// Feed the existing file input so limits, change hooks, manual uploads and refs
// behave exactly as they do when a user selects a file from the chooser.
export default {
  mounted(el, binding) {
    const button = document.createElement('button')
    button.type = 'button'
    button.className = 'paste-image-tip'
    button.textContent = '点击此处后按 Ctrl+V / ⌘V 粘贴图片'
    const state = { enabled: binding.value !== false, button }
    const getInput = () => el.querySelector('input[type="file"]')
    const isDisabled = () => {
      const input = getInput()
      const trigger = el.querySelector('.el-upload')
      // Some pages disable only the chooser button instead of el-upload.
      const chooser = trigger?.querySelector('button')
      return !state.enabled || !input || input.disabled ||
        trigger?.getAttribute('aria-disabled') === 'true' || Boolean(chooser?.disabled)
    }
    state.sync = () => {
      button.hidden = !state.enabled
      button.disabled = isDisabled()
      if (!el.contains(button)) el.appendChild(button)
    }
    state.paste = (event) => {
      if (event.defaultPrevented || isDisabled()) return
      if (event.target.closest('textarea, [contenteditable="true"], input:not([type="file"])')) return
      const files = getClipboardImages(event)
      if (!files.length) return
      const input = getInput()
      const transfer = new DataTransfer()
      files.forEach((file) => transfer.items.add(file))
      event.preventDefault()
      event.stopPropagation()
      input.files = transfer.files
      input.dispatchEvent(new Event('change', { bubbles: true }))
    }
    state.focus = (event) => {
      event.stopPropagation()
      button.focus()
    }
    button.addEventListener('click', state.focus)
    el.addEventListener('paste', state.paste)
    states.set(el, state)
    state.sync()
  },
  updated(el, binding) {
    const state = states.get(el)
    if (!state) return
    state.enabled = binding.value !== false
    state.sync()
  },
  beforeUnmount(el) {
    const state = states.get(el)
    if (!state) return
    el.removeEventListener('paste', state.paste)
    state.button.removeEventListener('click', state.focus)
    state.button.remove()
    states.delete(el)
  }
}
