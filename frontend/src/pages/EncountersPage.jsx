import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { campaignsApi, encountersApi } from '../api/campaign'
import ErrorMessage from '../components/ErrorMessage'
import { useFetch } from '../hooks/useFetch'

function EncountersPage() {
  const navigate = useNavigate()
  const { data: encounters, error: loadError, reload } = useFetch(() => encountersApi.list())
  const { data: campaigns } = useFetch(() => campaignsApi.list())
  const [form, setForm] = useState({ name: '', campaignId: '' })
  const [error, setError] = useState(null)

  const handleCreate = async (event) => {
    event.preventDefault()
    setError(null)
    try {
      const created = await encountersApi.create({
        name: form.name,
        campaignId: form.campaignId === '' ? null : Number(form.campaignId),
      })
      navigate(`/encounters/${created.id}`)
    } catch (err) {
      setError(err)
    }
  }

  const handleDelete = async (encounter) => {
    if (!window.confirm(`Delete encounter "${encounter.name}"?`)) return
    await encountersApi.remove(encounter.id)
    reload()
  }

  return (
    <>
      <div className="page-header">
        <h1>Encounters</h1>
      </div>

      <ErrorMessage error={loadError} />

      <form className="card form" onSubmit={handleCreate}>
        <div className="row">
          <label className="spacer">
            New encounter
            <input
              value={form.name}
              onChange={(e) => setForm({ ...form, name: e.target.value })}
              placeholder="Goblin ambush"
              required
            />
          </label>
          <label>
            Campaign
            <select value={form.campaignId} onChange={(e) => setForm({ ...form, campaignId: e.target.value })}>
              <option value="">– none –</option>
              {campaigns?.map((c) => (
                <option key={c.id} value={c.id}>{c.name}</option>
              ))}
            </select>
          </label>
          <button type="submit" className="align-end">Create</button>
        </div>
        <ErrorMessage error={error} />
      </form>

      <table>
        <thead>
          <tr>
            <th>Name</th>
            <th className="number">Combatants</th>
            <th className="number">Round</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {encounters?.map((encounter) => (
            <tr key={encounter.id}>
              <td>
                <Link to={`/encounters/${encounter.id}`}>{encounter.name}</Link>
              </td>
              <td className="number">{encounter.combatantCount}</td>
              <td className="number">{encounter.round === 0 ? 'not started' : encounter.round}</td>
              <td className="number">
                <button className="small danger" onClick={() => handleDelete(encounter)}>Delete</button>
              </td>
            </tr>
          ))}
          {encounters?.length === 0 && (
            <tr>
              <td colSpan="4" className="muted">No encounters yet.</td>
            </tr>
          )}
        </tbody>
      </table>
    </>
  )
}

export default EncountersPage
