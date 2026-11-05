function HpBar({ current, max }) {
  const percent = max > 0 ? Math.max(0, Math.min(100, (current / max) * 100)) : 0

  return (
    <div className={`hp-bar ${percent <= 25 ? 'low' : ''}`} title={`${current} / ${max} HP`}>
      <div style={{ width: `${percent}%` }} />
    </div>
  )
}

export default HpBar
