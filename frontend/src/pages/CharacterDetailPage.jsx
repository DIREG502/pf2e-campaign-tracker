import { Link, useNavigate, useParams } from 'react-router-dom'
import { charactersApi } from '../api/campaign'
import ErrorMessage from '../components/ErrorMessage'
import HpBar from '../components/HpBar'
import InventoryTable from '../components/InventoryTable'
import { useFetch } from '../hooks/useFetch'
import { formatModifier } from '../utils/format'

const ABILITIES = [
  ['STR', 'strength'],
  ['DEX', 'dexterity'],
  ['CON', 'constitution'],
  ['INT', 'intelligence'],
  ['WIS', 'wisdom'],
  ['CHA', 'charisma'],
]

function CharacterDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { data: character, error } = useFetch(() => charactersApi.get(id), [id])

  const handleDelete = async () => {
    if (!window.confirm(`Delete ${character.name} and all items?`)) return
    await charactersApi.remove(id)
    navigate('/characters')
  }

  if (error) return <ErrorMessage error={error} />
  if (!character) return <p className="muted">Loading…</p>

  return (
    <>
      <div className="page-header">
        <div>
          <h1>{character.name}</h1>
          <p className="muted">
            Level {character.level} {character.ancestry} {character.class}
            {character.campaignName && ` · ${character.campaignName}`}
          </p>
        </div>
        <div className="row">
          <Link to={`/characters/${id}/edit`}>
            <button className="secondary">Edit</button>
          </Link>
          <button className="danger" onClick={handleDelete}>Delete</button>
        </div>
      </div>

      <div className="card">
        <div className="row">
          <span className="stat">
            <span className="label">AC</span>
            <span className="value">{character.armorClass}</span>
          </span>
          <span className="stat">
            <span className="label">HP</span>
            <span className="value">
              {character.currentHp}/{character.maxHp}
            </span>
          </span>
          {ABILITIES.map(([label, key]) => (
            <span className="stat" key={key}>
              <span className="label">{label}</span>
              <span className="value">{formatModifier(character[key])}</span>
            </span>
          ))}
        </div>
        <div style={{ marginTop: '0.75rem' }}>
          <HpBar current={character.currentHp} max={character.maxHp} />
        </div>
        {character.notes && <p className="notes">{character.notes}</p>}
      </div>

      <InventoryTable characterId={id} />
    </>
  )
}

export default CharacterDetailPage
