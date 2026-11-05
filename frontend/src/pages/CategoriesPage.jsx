import { useState } from 'react'
import { categoriesApi } from '../api/expenses'
import ErrorMessage from '../components/ErrorMessage'
import { useFetch } from '../hooks/useFetch'

const EMPTY_CATEGORY = { name: '', color: '#4e79a7' }

function CategoriesPage() {
  const { data: categories, error: loadError, reload } = useFetch(() => categoriesApi.list())
  const [form, setForm] = useState(EMPTY_CATEGORY)
  const [editingId, setEditingId] = useState(null)
  const [error, setError] = useState(null)

  const handleSubmit = async (event) => {
    event.preventDefault()
    setError(null)
    try {
      if (editingId) {
        await categoriesApi.update(editingId, form)
      } else {
        await categoriesApi.create(form)
      }
      setForm(EMPTY_CATEGORY)
      setEditingId(null)
      reload()
    } catch (err) {
      setError(err)
    }
  }

  const handleDelete = async (category) => {
    if (!window.confirm(`Delete category "${category.name}"?`)) return
    setError(null)
    try {
      await categoriesApi.remove(category.id)
      reload()
    } catch (err) {
      setError(err)
    }
  }

  return (
    <>
      <div className="page-header">
        <h1>Categories</h1>
      </div>

      <ErrorMessage error={loadError} />

      <form className="card form" onSubmit={handleSubmit}>
        <div className="row">
          <label className="spacer">
            {editingId ? 'Edit category' : 'New category'}
            <input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
          </label>
          <label>
            Color
            <input type="color" value={form.color} onChange={(e) => setForm({ ...form, color: e.target.value })} />
          </label>
          <button type="submit" className="align-end">{editingId ? 'Save' : 'Add'}</button>
          {editingId && (
            <button
              type="button"
              className="secondary align-end"
              onClick={() => {
                setEditingId(null)
                setForm(EMPTY_CATEGORY)
              }}
            >
              Cancel
            </button>
          )}
        </div>
        <ErrorMessage error={error} />
      </form>

      <table>
        <thead>
          <tr>
            <th>Name</th>
            <th className="number">Expenses</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {categories?.map((category) => (
            <tr key={category.id}>
              <td>
                <span className="swatch" style={{ background: category.color }} />
                {category.name}
              </td>
              <td className="number">{category.expenseCount}</td>
              <td className="number">
                <button
                  className="small secondary"
                  onClick={() => {
                    setEditingId(category.id)
                    setForm({ name: category.name, color: category.color })
                  }}
                >
                  Edit
                </button>{' '}
                <button className="small danger" onClick={() => handleDelete(category)}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  )
}

export default CategoriesPage
