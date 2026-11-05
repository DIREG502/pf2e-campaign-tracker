import { useState } from 'react'
import { Link } from 'react-router-dom'
import { categoriesApi, expensesApi } from '../api/expenses'
import ErrorMessage from '../components/ErrorMessage'
import ExpenseFilterBar from '../components/ExpenseFilterBar'
import ExpenseForm from '../components/ExpenseForm'
import { useExpenseFilter } from '../hooks/useExpenseFilter'
import { useFetch } from '../hooks/useFetch'
import { formatDate, formatMoney } from '../utils/format'

function ExpensesPage() {
  const [filter, setFilter, filterKey] = useExpenseFilter()
  const { data: categories } = useFetch(() => categoriesApi.list())
  const { data: expenses, error, reload } = useFetch(() => expensesApi.list(filter), [filterKey])
  const [editing, setEditing] = useState(null)
  const [actionError, setActionError] = useState(null)

  const total = expenses?.reduce((sum, e) => sum + e.amount, 0) ?? 0

  const handleCreate = async (data) => {
    await expensesApi.create(data)
    reload()
  }

  const handleUpdate = async (data) => {
    await expensesApi.update(editing.id, data)
    setEditing(null)
    reload()
  }

  // Exports exactly what the table shows: the backend applies the same filter.
  const handleExport = async () => {
    setActionError(null)
    try {
      await expensesApi.exportCsv(filter)
    } catch (err) {
      setActionError(err)
    }
  }

  const handleDelete = async (expense) => {
    if (!window.confirm(`Delete "${expense.description}"?`)) return
    await expensesApi.remove(expense.id)
    reload()
  }

  return (
    <>
      <div className="page-header">
        <h1>Expenses</h1>
        <div className="row">
          <Link to="/expenses/categories">Manage categories</Link>
          <button className="secondary" onClick={handleExport} disabled={!expenses?.length}>
            Export CSV
          </button>
        </div>
      </div>

      <ExpenseFilterBar filter={filter} onChange={setFilter} categories={categories} />

      {categories &&
        (editing ? (
          <ExpenseForm
            key={editing.id}
            expense={editing}
            categories={categories}
            onSave={handleUpdate}
            onCancel={() => setEditing(null)}
          />
        ) : (
          <ExpenseForm categories={categories} onSave={handleCreate} />
        ))}

      <ErrorMessage error={error || actionError} />

      <table>
        <thead>
          <tr>
            <th>Date</th>
            <th>Description</th>
            <th>Category</th>
            <th className="number">Amount</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {expenses?.map((expense) => (
            <tr key={expense.id}>
              <td>{formatDate(expense.date)}</td>
              <td>{expense.description}</td>
              <td>
                <span className="swatch" style={{ background: expense.categoryColor }} />
                {expense.categoryName}
              </td>
              <td className="number">{formatMoney(expense.amount)}</td>
              <td className="number">
                <button className="small secondary" onClick={() => setEditing(expense)}>Edit</button>{' '}
                <button className="small danger" onClick={() => handleDelete(expense).catch(setActionError)}>
                  Delete
                </button>
              </td>
            </tr>
          ))}
          {expenses?.length === 0 && (
            <tr>
              <td colSpan="5" className="muted">No expenses in this range.</td>
            </tr>
          )}
        </tbody>
        <tfoot>
          <tr>
            <td colSpan="3">Total ({expenses?.length ?? 0} expenses)</td>
            <td className="number">{formatMoney(total)}</td>
            <td></td>
          </tr>
        </tfoot>
      </table>
    </>
  )
}

export default ExpensesPage
