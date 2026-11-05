function startOfMonth(offsetMonths = 0) {
  const now = new Date()
  const date = new Date(now.getFullYear(), now.getMonth() + offsetMonths, 1)
  return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-01`
}

function ExpenseFilterBar({ filter, onChange, categories, showCategory = true }) {
  return (
    <div className="card filter-bar">
      <div className="row">
        <label>
          From
          <input type="date" value={filter.from} max={filter.to || undefined} onChange={(e) => onChange({ from: e.target.value })} />
        </label>
        <label>
          To
          <input type="date" value={filter.to} min={filter.from || undefined} onChange={(e) => onChange({ to: e.target.value })} />
        </label>
        {showCategory && (
          <label>
            Category
            <select value={filter.categoryId} onChange={(e) => onChange({ categoryId: e.target.value })}>
              <option value="">All categories</option>
              {categories?.map((c) => (
                <option key={c.id} value={c.id}>{c.name}</option>
              ))}
            </select>
          </label>
        )}
        <div className="row presets">
          <button type="button" className="small secondary" onClick={() => onChange({ from: startOfMonth(), to: '' })}>
            This month
          </button>
          <button type="button" className="small secondary" onClick={() => onChange({ from: startOfMonth(-5), to: '' })}>
            Last 6 months
          </button>
          <button type="button" className="small secondary" onClick={() => onChange({ from: '', to: '', categoryId: '' })}>
            Clear
          </button>
        </div>
      </div>
    </div>
  )
}

export default ExpenseFilterBar
