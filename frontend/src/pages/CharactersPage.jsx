import { Link } from 'react-router-dom'
import { charactersApi } from '../api/campaign'
import ErrorMessage from '../components/ErrorMessage'
import HpBar from '../components/HpBar'
import { useFetch } from '../hooks/useFetch'

function CharactersPage() {
  const { data: characters, error, loading } = useFetch(() => charactersApi.list())

  return (
    <>
      <div className="page-header">
        <h1>Characters</h1>
        <Link to="/characters/new">
          <button>New character</button>
        </Link>
      </div>

      <ErrorMessage error={error} />
      {loading && <p className="muted">Loading…</p>}
      {characters?.length === 0 && <p className="muted">No characters yet. Create your first one!</p>}

      <div className="grid">
        {characters?.map((character) => (
          <Link key={character.id} to={`/characters/${character.id}`} className="card character-card">
            <h3>{character.name}</h3>
            <p className="muted">
              Level {character.level} {character.ancestry} {character.class}
            </p>
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
              <span className="stat">
                <span className="label">Items</span>
                <span className="value">{character.itemCount}</span>
              </span>
            </div>
            <HpBar current={character.currentHp} max={character.maxHp} />
            {character.campaignName && <p className="muted small">{character.campaignName}</p>}
          </Link>
        ))}
      </div>
    </>
  )
}

export default CharactersPage
