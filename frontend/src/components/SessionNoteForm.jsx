import { useState } from 'react'
import { todayIso } from '../utils/format'
import ErrorMessage from './ErrorMessage'

function toFormState(note) {
  if (!note) return { title: '', sessionDate: todayIso(), content: '', tags: '' }
  return { ...note, tags: note.tags.join(', ') }
}

/** Create/edit form for a session note. Tags are typed as a comma-separated list. */
function SessionNoteForm({ note, onSave, onCancel }) {
  const [form, setForm] = useState(() => toFormState(note))
  const [error, setError] = useState(null)
  const [saving, setSaving] = useState(false)

  const update = (field) => (event) => setForm({ ...form, [field]: event.target.value })

  const handleSubmit = async (event) => {
    event.preventDefault()
    setSaving(true)
    setError(null)
    try {
      await onSave({
        title: form.title,
        sessionDate: form.sessionDate,
        content: form.content,
        tags: form.tags.split(',').map((t) => t.trim()).filter(Boolean),
      })
    } catch (err) {
      setError(err)
      setSaving(false)
    }
  }

  return (
    <form className="card form" onSubmit={handleSubmit}>
      <h2>{note ? 'Edit note' : 'New session note'}</h2>
      <div className="form-grid">
        <label>
          Title
          <input value={form.title} onChange={update('title')} required />
        </label>
        <label>
          Session date
          <input type="date" value={form.sessionDate} onChange={update('sessionDate')} required />
        </label>
        <label>
          Tags <span className="muted">(comma separated)</span>
          <input value={form.tags} onChange={update('tags')} placeholder="combat, loot" />
        </label>
      </div>
      <label>
        Notes <span className="muted">(Markdown supported)</span>
        <textarea value={form.content} onChange={update('content')} rows={10} />
      </label>
      <ErrorMessage error={error} />
      <div className="row">
        <button type="submit" disabled={saving}>{saving ? 'Saving…' : 'Save note'}</button>
        <button type="button" className="secondary" onClick={onCancel}>Cancel</button>
      </div>
    </form>
  )
}

export default SessionNoteForm
