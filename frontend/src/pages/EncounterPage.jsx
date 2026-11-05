import { useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { charactersApi, encountersApi } from '../api/campaign'
import CombatantRow from '../components/CombatantRow'
import ErrorMessage from '../components/ErrorMessage'
import { useFetch } from '../hooks/useFetch'

const EMPTY_NPC = { name: '', initiative: 10, currentHp: 20, maxHp: 20 }

/** The initiative tracker for one encounter. */
function EncounterPage() {
  const { id } = useParams()
  const { data: encounter, error: loadError, setData: setEncounter } = useFetch(() => encountersApi.get(id), [id])
  const { data: characters } = useFetch(() => charactersApi.list())
  const [npc, setNpc] = useState(EMPTY_NPC)
  const [pc, setPc] = useState({ characterId: '', initiative: 10 })
  const [error, setError] = useState(null)

  // Every combat endpoint returns the updated encounter, so we just replace our copy.
  const run = async (action) => {
    setError(null)
    try {
      setEncounter(await action())
    } catch (err) {
      setError(err)
    }
  }

  const addNpc = (event) => {
    event.preventDefault()
    run(() => encountersApi.addCombatant(id, { ...npc, conditions: [], isPlayerCharacter: false }))
    setNpc(EMPTY_NPC)
  }

  const addCharacter = (event) => {
    event.preventDefault()
    run(() => encountersApi.addCharacter(id, { characterId: Number(pc.characterId), initiative: pc.initiative }))
    setPc({ characterId: '', initiative: 10 })
  }

  if (loadError) return <ErrorMessage error={loadError} />
  if (!encounter) return <p className="muted">Loading…</p>

  const started = encounter.round > 0
  const current = encounter.combatants.find((c) => c.id === encounter.currentCombatantId)
  const usedCharacterIds = new Set(encounter.combatants.map((c) => c.characterId).filter(Boolean))

  return (
    <>
      <div className="page-header">
        <div>
          <Link to="/encounters" className="muted">← Encounters</Link>
          <h1>{encounter.name}</h1>
        </div>
        <div className="row">
          <div className="round-counter">
            <span className="label">Round</span>
            <span className="value">{started ? encounter.round : '–'}</span>
          </div>
          <button onClick={() => run(() => encountersApi.nextTurn(id))} disabled={encounter.combatants.length === 0}>
            {started ? 'Next turn' : 'Start combat'}
          </button>
          {started && (
            <button className="secondary" onClick={() => run(() => encountersApi.reset(id))}>Reset</button>
          )}
        </div>
      </div>

      {current && (
        <p className="current-banner">
          <strong>{current.name}</strong>'s turn
        </p>
      )}

      <ErrorMessage error={error} />

      <table className="initiative-table">
        <thead>
          <tr>
            <th></th>
            <th>Init</th>
            <th>Name</th>
            <th>HP</th>
            <th>Conditions</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {encounter.combatants.map((combatant) => (
            <CombatantRow
              key={combatant.id}
              combatant={combatant}
              isCurrent={combatant.id === encounter.currentCombatantId}
              onUpdate={(c, data) => run(() => encountersApi.updateCombatant(id, c.id, data))}
              onRemove={(c) => run(() => encountersApi.removeCombatant(id, c.id))}
            />
          ))}
          {encounter.combatants.length === 0 && (
            <tr>
              <td colSpan="6" className="muted">Add characters and monsters below.</td>
            </tr>
          )}
        </tbody>
      </table>

      <div className="grid add-combatants">
        <form className="card form" onSubmit={addCharacter}>
          <h3>Add character</h3>
          <label>
            Character
            <select value={pc.characterId} onChange={(e) => setPc({ ...pc, characterId: e.target.value })} required>
              <option value="">Choose…</option>
              {characters
                ?.filter((c) => !usedCharacterIds.has(c.id))
                .map((c) => (
                  <option key={c.id} value={c.id}>{c.name}</option>
                ))}
            </select>
          </label>
          <label>
            Initiative
            <input type="number" value={pc.initiative} onChange={(e) => setPc({ ...pc, initiative: Number(e.target.value) })} />
          </label>
          <div>
            <button type="submit">Add character</button>
          </div>
        </form>

        <form className="card form" onSubmit={addNpc}>
          <h3>Add NPC / monster</h3>
          <label>
            Name
            <input value={npc.name} onChange={(e) => setNpc({ ...npc, name: e.target.value })} required placeholder="Goblin Warrior" />
          </label>
          <div className="form-grid">
            <label>
              Initiative
              <input type="number" value={npc.initiative} onChange={(e) => setNpc({ ...npc, initiative: Number(e.target.value) })} />
            </label>
            <label>
              HP
              <input
                type="number"
                min="1"
                value={npc.maxHp}
                onChange={(e) => setNpc({ ...npc, maxHp: Number(e.target.value), currentHp: Number(e.target.value) })}
              />
            </label>
          </div>
          <div>
            <button type="submit">Add NPC</button>
          </div>
        </form>
      </div>
    </>
  )
}

export default EncounterPage
