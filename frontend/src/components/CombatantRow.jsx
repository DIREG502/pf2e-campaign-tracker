import { useEffect, useState } from 'react'
import HpBar from './HpBar'

/**
 * One row of the initiative table. Initiative, HP and conditions are edited
 * inline and saved when the field loses focus or Enter is pressed.
 */
function CombatantRow({ combatant, isCurrent, onUpdate, onRemove }) {
  const [initiative, setInitiative] = useState(combatant.initiative)
  const [hp, setHp] = useState(combatant.currentHp)
  const [conditions, setConditions] = useState(combatant.conditions.join(', '))

  // Keep the inputs in sync when the server sends new values (e.g. after the +/- buttons).
  useEffect(() => setInitiative(combatant.initiative), [combatant.initiative])
  useEffect(() => setHp(combatant.currentHp), [combatant.currentHp])
  const savedConditions = combatant.conditions.join(', ')
  useEffect(() => setConditions(savedConditions), [savedConditions])

  const save = (changes) => {
    onUpdate(combatant, {
      name: combatant.name,
      initiative: combatant.initiative,
      currentHp: combatant.currentHp,
      maxHp: combatant.maxHp,
      conditions: combatant.conditions,
      isPlayerCharacter: combatant.isPlayerCharacter,
      ...changes,
    })
  }

  const saveOnEnter = (event) => {
    if (event.key === 'Enter') event.target.blur()
  }

  const clampHp = (value) => Math.max(0, Math.min(combatant.maxHp, value))

  return (
    <tr className={isCurrent ? 'current-turn' : ''}>
      <td className="turn-marker">{isCurrent ? '▶' : ''}</td>
      <td>
        <input
          className="initiative-input"
          type="number"
          value={initiative}
          onChange={(e) => setInitiative(Number(e.target.value))}
          onBlur={() => initiative !== combatant.initiative && save({ initiative })}
          onKeyDown={saveOnEnter}
          aria-label={`Initiative of ${combatant.name}`}
        />
      </td>
      <td>
        <strong>{combatant.name}</strong>{' '}
        <span className={`badge ${combatant.isPlayerCharacter ? 'pc' : 'npc'}`}>
          {combatant.isPlayerCharacter ? 'PC' : 'NPC'}
        </span>
      </td>
      <td className="hp-cell">
        <div className="row">
          <button className="small secondary" onClick={() => save({ currentHp: clampHp(combatant.currentHp - 1) })}>−</button>
          <input
            className="hp-input"
            type="number"
            value={hp}
            onChange={(e) => setHp(Number(e.target.value))}
            onBlur={() => hp !== combatant.currentHp && save({ currentHp: clampHp(hp) })}
            onKeyDown={saveOnEnter}
            aria-label={`HP of ${combatant.name}`}
          />
          <span className="muted">/ {combatant.maxHp}</span>
          <button className="small secondary" onClick={() => save({ currentHp: clampHp(combatant.currentHp + 1) })}>+</button>
        </div>
        <HpBar current={combatant.currentHp} max={combatant.maxHp} />
      </td>
      <td>
        <input
          value={conditions}
          onChange={(e) => setConditions(e.target.value)}
          onBlur={() =>
            save({ conditions: conditions.split(',').map((c) => c.trim()).filter(Boolean) })
          }
          onKeyDown={saveOnEnter}
          placeholder="frightened 1, off-guard"
          aria-label={`Conditions of ${combatant.name}`}
        />
      </td>
      <td className="number">
        <button className="small danger" onClick={() => onRemove(combatant)}>Remove</button>
      </td>
    </tr>
  )
}

export default CombatantRow
