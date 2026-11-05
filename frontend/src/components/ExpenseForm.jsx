import { useState } from 'react'
import { todayIso } from '../utils/format'
import ErrorMessage from './ErrorMessage'

function toFormState(expense, categories) {
  if (expense) return { ...expense }
  return { amount: '', date: todayIso(), description: '', categoryId: categories?.[0]?.id ?? '' }
}

function ExpenseForm({ expense, categories, onSave, onCancel }) {
  const [form, setForm] = useState(() => toFormState(expense, categories))
  const [error, setError] = useState(null)

  const update = (field) => (event) => setForm({ ...form, [field]: event.target.value })

  const handleSubmit = async (event) => {
    event.preventDefault()
    setError(null)
    try {
      await onSave({
        amount: Number(form.amount),
        date: form.date,
        description: form.description,
        categoryId: Number(form.categoryId),
      })
      if (!expense) setForm(toFormState(null, categories))
    } catch (err) {
      setError(err)
    }
  }

  return (
    <form className="card form" onSubmit={handleSubmit}>
      <h3>{expense ? 'Edit expense' : 'Add expense'}</h3>
      <div className="form-grid">
        <label>
          Amount (€)
          <input type="number" min="0.01" step="0.01" value={form.amount} onChange={update('amount')} required />
        </label>
        <label>
          Date
          <input type="date" value={form.date} onChange={update('date')} required />
        </label>
        <label>
          Category
          <select value={form.categoryId} onChange={update('categoryId')} required>
            {categories?.map((c) => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>
        </label>
      </div>
      <label>
        Description
        <input value={form.description} onChange={update('description')} required />
      </label>
      <ErrorMessage error={error} />
      <div className="row">
        <button type="submit">{expense ? 'Save' : 'Add expense'}</button>
        {onCancel && (
          <button type="button" className="secondary" onClick={onCancel}>Cancel</button>
        )}
      </div>
    </form>
  )
}

export default ExpenseForm
