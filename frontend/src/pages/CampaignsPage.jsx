import { useState } from 'react'
import { Link } from 'react-router-dom'
import { campaignsApi } from '../api/campaign'
import ErrorMessage from '../components/ErrorMessage'
import { useFetch } from '../hooks/useFetch'

function CampaignsPage() {
  const { data: campaigns, error: loadError, reload } = useFetch(() => campaignsApi.list())
  const [form, setForm] = useState({ name: '', description: '' })
  const [error, setError] = useState(null)

  const handleCreate = async (event) => {
    event.preventDefault()
    setError(null)
    try {
      await campaignsApi.create(form)
      setForm({ name: '', description: '' })
      reload()
    } catch (err) {
      setError(err)
    }
  }

  const handleDelete = async (campaign) => {
    if (!window.confirm(`Delete "${campaign.name}" and all its session notes? Characters are kept.`)) return
    try {
      await campaignsApi.remove(campaign.id)
      reload()
    } catch (err) {
      setError(err)
    }
  }

  return (
    <>
      <div className="page-header">
        <h1>Campaigns</h1>
      </div>

      <ErrorMessage error={loadError} />

      <div className="grid">
        {campaigns?.map((campaign) => (
          <div key={campaign.id} className="card">
            <h3>
              <Link to={`/campaigns/${campaign.id}`}>{campaign.name}</Link>
            </h3>
            {campaign.description && <p>{campaign.description}</p>}
            <div className="row">
              <span className="muted">{campaign.sessionNoteCount} session notes</span>
              <span className="spacer" />
              <button className="small danger" onClick={() => handleDelete(campaign)}>Delete</button>
            </div>
          </div>
        ))}
      </div>

      <form className="card form" onSubmit={handleCreate}>
        <h2>New campaign</h2>
        <label>
          Name
          <input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
        </label>
        <label>
          Description
          <input value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
        </label>
        <ErrorMessage error={error} />
        <div>
          <button type="submit">Create campaign</button>
        </div>
      </form>
    </>
  )
}

export default CampaignsPage
