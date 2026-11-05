import { useState } from 'react'
import Markdown from 'react-markdown'
import { Link, useParams } from 'react-router-dom'
import { campaignsApi, notesApi } from '../api/campaign'
import ErrorMessage from '../components/ErrorMessage'
import SessionNoteForm from '../components/SessionNoteForm'
import { useFetch } from '../hooks/useFetch'
import { formatDate } from '../utils/format'

function CampaignNotesPage() {
  const { id } = useParams()
  const [tag, setTag] = useState('')
  const [editing, setEditing] = useState(null) // null = closed, 'new' or a note object
  const { data: campaign, error: campaignError } = useFetch(() => campaignsApi.get(id), [id])
  const { data: notes, error, reload } = useFetch(() => notesApi.list(id, tag), [id, tag])
  // Tag buttons come from the unfiltered list so they don't disappear while filtering.
  const { data: allNotes, reload: reloadAll } = useFetch(() => notesApi.list(id), [id])

  const allTags = [...new Set(allNotes?.flatMap((n) => n.tags) ?? [])].sort()

  const handleSave = async (data) => {
    if (editing === 'new') {
      await notesApi.create(id, data)
    } else {
      await notesApi.update(id, editing.id, data)
    }
    setEditing(null)
    reload()
    reloadAll()
  }

  const handleDelete = async (note) => {
    if (!window.confirm(`Delete "${note.title}"?`)) return
    await notesApi.remove(id, note.id)
    reload()
    reloadAll()
  }

  if (campaignError) return <ErrorMessage error={campaignError} />

  return (
    <>
      <div className="page-header">
        <div>
          <Link to="/campaigns" className="muted">← Campaigns</Link>
          <h1>{campaign?.name ?? '…'}</h1>
          {campaign?.description && <p className="muted">{campaign.description}</p>}
        </div>
        {!editing && <button onClick={() => setEditing('new')}>New note</button>}
      </div>

      {editing && (
        <SessionNoteForm
          key={editing === 'new' ? 'new' : editing.id}
          note={editing === 'new' ? null : editing}
          onSave={handleSave}
          onCancel={() => setEditing(null)}
        />
      )}

      {allTags.length > 0 && (
        <div className="row tag-filter">
          <span className="muted">Filter:</span>
          <button className={`tag ${tag === '' ? 'active' : ''}`} onClick={() => setTag('')}>all</button>
          {allTags.map((t) => (
            <button key={t} className={`tag ${tag === t ? 'active' : ''}`} onClick={() => setTag(t)}>
              {t}
            </button>
          ))}
        </div>
      )}

      <ErrorMessage error={error} />
      {notes?.length === 0 && <p className="muted">No session notes yet.</p>}

      {notes?.map((note) => (
        <article key={note.id} className="card note">
          <div className="row">
            <h2>{note.title}</h2>
            <span className="muted">{formatDate(note.sessionDate)}</span>
            <span className="spacer" />
            <button className="small secondary" onClick={() => setEditing(note)}>Edit</button>
            <button className="small danger" onClick={() => handleDelete(note)}>Delete</button>
          </div>
          <div>
            {note.tags.map((t) => (
              <span key={t} className="tag">{t}</span>
            ))}
          </div>
          <div className="markdown">
            <Markdown>{note.content}</Markdown>
          </div>
        </article>
      ))}
    </>
  )
}

export default CampaignNotesPage
