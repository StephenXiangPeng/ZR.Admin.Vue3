// Run with: node --test scripts/test-image-paste.cjs
const { test, after } = require('node:test')
const assert = require('node:assert/strict')
const fs = require('node:fs')
const os = require('node:os')
const path = require('node:path')
const { JSDOM } = require('jsdom')
const { buildSync } = require('esbuild')
const dom = new JSDOM('<!doctype html><html><body></body></html>', { url: 'http://localhost' })
for (const key of ['window', 'document', 'navigator', 'Element', 'HTMLElement', 'SVGElement', 'HTMLInputElement', 'Event', 'File', 'Blob', 'FileReader', 'MutationObserver']) {
  Object.defineProperty(globalThis, key, { value: dom.window[key], configurable: true })
}
globalThis.getComputedStyle = dom.window.getComputedStyle
globalThis.requestAnimationFrame = callback => setTimeout(callback, 0)
globalThis.cancelAnimationFrame = clearTimeout
globalThis.URL.createObjectURL = () => 'blob:test-image'
globalThis.URL.revokeObjectURL = () => {}
// jsdom has no OS clipboard/DataTransfer. Mock only this browser boundary.
globalThis.DataTransfer = class {
  files = []
  items = { add: (file) => this.files.push(file) }
}
const inputFiles = new WeakMap()
Object.defineProperty(HTMLInputElement.prototype, 'files', {
  configurable: true,
  get() { return inputFiles.get(this) || [] },
  set(files) { inputFiles.set(this, files) }
})
const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'image-paste-test-'))
for (const [name, entry] of [['directive', 'src/directive/module/pasteImage.js'], ['clipboard', 'src/utils/clipboardImage.js']]) {
  buildSync({ entryPoints: [path.resolve(__dirname, '..', entry)], bundle: true, platform: 'node', format: 'cjs', loader: { '.css': 'empty' }, outfile: path.join(dir, name + '.cjs') })
}
const directive = require(path.join(dir, 'directive.cjs')).default
const { enableQuillImagePaste } = require(path.join(dir, 'clipboard.cjs'))
const { createApp, h, withDirectives, ref, nextTick } = require('vue')
const { ElUpload, ElForm } = require('element-plus')
const apps = []
after(() => { apps.forEach(app => app.unmount()); dom.window.close(); fs.rmSync(dir, { recursive: true, force: true }) })
const image = () => new File(['image'], 'screenshot.png', { type: 'image/png' })
const flush = async () => { await nextTick(); await new Promise(resolve => setImmediate(resolve)); await nextTick() }
function mount(props = {}, options = {}) {
  const host = document.createElement('div')
  document.body.append(host)
  const changes = []
  const upload = ref()
  const enabled = ref(true)
  const render = () => withDirectives(h(ElUpload, { autoUpload: false, onChange: file => changes.push(file), ...props, ref: upload }, { default: () => h('button', { disabled: options.buttonDisabled }, 'Choose') }), [[directive, enabled.value]])
  const app = createApp({ render: () => options.formDisabled ? h(ElForm, { disabled: true }, { default: render }) : render() })
  app.mount(host)
  apps.push(app)
  return { host, changes, upload, enabled, app, button: host.querySelector('.paste-image-tip') }
}
function paste(target, files = [image()]) {
  const event = new Event('paste', { bubbles: true, cancelable: true })
  Object.defineProperty(event, 'clipboardData', { value: { items: files.map(file => ({ kind: 'file', getAsFile: () => file })), files } })
  target.dispatchEvent(event)
  return event
}
test('paste is local to the focused uploader and uses the real manual change pipeline', async () => {
  const a = mount(), b = mount()
  a.button.click()
  assert.equal(document.activeElement, a.button)
  assert.equal(paste(a.button).defaultPrevented, true)
  await flush()
  assert.equal(a.changes.length, 1)
  assert.equal(a.changes[0].raw.name, 'screenshot.png')
  assert.equal(a.changes[0].status, 'ready')
  assert.equal(b.changes.length, 0)
})
test('ordinary text/non-image paste is left alone', () => {
  const a = mount()
  assert.equal(paste(a.button, [new File(['text'], 'note.txt', { type: 'text/plain' })]).defaultPrevented, false)
  assert.equal(a.changes.length, 0)
})
test('disabled upload, form, chooser and permission binding prevent changes', async () => {
  for (const a of [mount({ disabled: true }), mount({}, { formDisabled: true }), mount({}, { buttonDisabled: true })]) {
    paste(a.button)
    assert.equal(a.changes.length, 0)
    assert.equal(a.button.disabled, true)
  }
  const a = mount()
  a.enabled.value = false
  await flush()
  paste(a.button)
  assert.equal(a.changes.length, 0)
  assert.equal(a.button.hidden, true)
})
test('real Element Plus limit handler rejects excess images', async () => {
  let exceeded = 0
  const a = mount({ multiple: true, limit: 1, onExceed: () => exceeded++ })
  paste(a.button, [image(), image()])
  await flush()
  assert.equal(exceeded, 1)
  assert.equal(a.changes.length, 0)
})
test('multiple images and subsequent paste preserve file-list updates', async () => {
  const a = mount({ multiple: true, limit: 3 })
  paste(a.button, [image(), image()])
  await flush()
  paste(a.button)
  await flush()
  assert.equal(a.changes.length, 3)
  assert.equal(new Set(a.changes.map(file => file.uid)).size, 3)
  assert.equal(a.host.querySelectorAll('.paste-image-tip').length, 1)
})
test('auto upload runs validation and the existing custom request with its headers', async () => {
  const requests = []
  let validated = 0
  const a = mount({ autoUpload: true, headers: { Authorization: 'test' }, beforeUpload: () => { validated++; return true }, httpRequest: options => { requests.push(options); return Promise.resolve({ code: 200 }) } })
  paste(a.button)
  await flush()
  assert.equal(validated, 1)
  assert.equal(requests.length, 1)
  assert.equal(requests[0].headers.Authorization, 'test')
})
test('validation rejection prevents requests, and manual uploads wait for submit', async () => {
  let requests = 0
  const a = mount({ autoUpload: true, beforeUpload: () => false, httpRequest: () => { requests++ } })
  paste(a.button)
  await flush()
  assert.equal(requests, 0)
  const b = mount({ httpRequest: () => { requests++; return Promise.resolve({}) } })
  paste(b.button)
  await flush()
  assert.equal(requests, 0)
  b.upload.value.submit()
  await flush()
  assert.equal(requests, 1)
})
test('unmount removes paste listeners', () => {
  const a = mount()
  const root = a.button.parentElement
  a.app.unmount()
  assert.equal(paste(root).defaultPrevented, false)
})
test('Quill image paste inserts once at selection, leaves text alone and cleans up', async () => {
  const root = document.createElement('div')
  document.body.append(root)
  const updates = []
  const quill = { root, isEnabled: () => true, getSelection: () => ({ index: 2, length: 1 }), getLength: () => 6, updateContents: (delta, source) => updates.push({ delta, source }), setSelection: () => {} }
  const cleanup = enableQuillImagePaste(quill)
  assert.equal(paste(root, []).defaultPrevented, false)
  assert.equal(paste(root).defaultPrevented, true)
  await new Promise(resolve => setTimeout(resolve, 30))
  assert.equal(updates.length, 1)
  assert.equal(updates[0].source, 'user')
  assert.deepEqual(updates[0].delta.ops.slice(0, 2), [{ retain: 2 }, { delete: 1 }])
  assert.match(updates[0].delta.ops[2].insert.image, /^data:image\/png;base64,/)
  cleanup()
  assert.equal(paste(root).defaultPrevented, false)
})
