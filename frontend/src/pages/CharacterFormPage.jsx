import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { campaignsApi, charactersApi } from '../api/campaign'
import ErrorMessage from '../components/ErrorMessage'
import { useFetch } from '../hooks/useFetch'

const ABILITIES = ['strength', 'dexterity', 'constitution', 'intelligence', 'wisdom', 'charisma']

const EMPTY_CHARACTER = {
  name: '',
  ancestry: '',
  class: '',
  level: 1,
  currentHp: 10,
  maxHp: 10,
  armorClass: 15,
  strength: 0,
  dexterity: 0,
  constitution: 0,
  intelligence: 0,
  wisdom: 0,
  charisma: 0,
  notes: '',
  campaignId: '',
}

function CharacterFormPage() {
  const { id } = useParams()
  const isEdit = Boolean(id)
  const navigate = useNavigate()
  const [form, setForm] = useState(EMPTY_CHARACTER)
  const [error, setError] = useState(null)
  const [saving, setSaving] = useState(false)
  const { data: campaigns } = useFetch(() => campaignsApi.list())

  useEffect(() => {
    if (!isEdit) return
    charactersApi
      .get(id)
      .then((character) => setForm({ ...character, notes: character.notes ?? '', campaignId: character.campaignId ?? '' }))
      .catch(setError)
  }, [id, isEdit])

  const update = (field, isNumber = false) => (event) => {
    const value = isNumber ? Number(event.target.value) : event.target.value
    setForm({ ...form, [field]: value })
  }

  const handleSubmit = async (event) => {
    event.preventDefault()
    setSaving(true)
    setError(null)

    const payload = { ...form, campaignId: form.campaignId === '' ? null : Number(form.campaignId) }
    try {
      if (isEdit) {
        await charactersApi.update(id, payload)
        navigate(`/characters/${id}`)
      } else {
        const created = await charactersApi.create(payload)
        navigate(`/characters/${created.id}`)
      }
    } catch (err) {
      setError(err)
      setSaving(false)
    }
  }

  return (
    <div className="card">
      <h1>{isEdit ? `Edit ${form.name}` : 'New character'}</h1>
      <form className="form" onSubmit={handleSubmit}>
        <div className="form-grid">
          <label>
            Name
            <input value={form.name} onChange={update('name')} required />
          </label>
          <label>
            Ancestry
            <input value={form.ancestry} onChange={update('ancestry')} required placeholder="Human, Elf, …" />
          </label>
          <label>
            Class
            <input value={form.class} onChange={update('class')} required placeholder="Fighter, Wizard, …" />
          </label>
          <label>
            Level
            <input type="number" min="1" max="20" value={form.level} onChange={update('level', true)} />
          </label>
          <label>
            Campaign
            <select value={form.campaignId} onChange={update('campaignId')}>
              <option value="">– none –</option>
              {campaigns?.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
          </label>
        </div>

        <div className="form-grid">
          <label>
            Current HP
            <input type="number" min="0" value={form.currentHp} onChange={update('currentHp', true)} />
          </label>
          <label>
            Max HP
            <input type="number" min="1" value={form.maxHp} onChange={update('maxHp', true)} />
          </label>
          <label>
            AC
            <input type="number" min="0" value={form.armorClass} onChange={update('armorClass', true)} />
          </label>
        </div>

        <fieldset className="form-grid">
          <legend>Ability modifiers</legend>
          {ABILITIES.map((ability) => (
            <label key={ability}>
              {ability.slice(0, 3).toUpperCase()}
              <input type="number" min="-5" max="10" value={form[ability]} onChange={update(ability, true)} />
            </label>
          ))}
        </fieldset>

        <label>
          Notes
          <textarea value={form.notes} onChange={update('notes')} />
        </label>

        <ErrorMessage error={error} />

        <div className="row">
          <button type="submit" disabled={saving}>
            {saving ? 'Saving…' : 'Save'}
          </button>
          <button type="button" className="secondary" onClick={() => navigate(-1)}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  )
}

export default CharacterFormPage
