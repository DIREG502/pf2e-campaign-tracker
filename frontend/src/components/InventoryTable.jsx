import { useState } from 'react'
import { inventoryApi } from '../api/campaign'
import { useFetch } from '../hooks/useFetch'
import { formatBulk } from '../utils/format'
import ErrorMessage from './ErrorMessage'

const EMPTY_ITEM = { name: '', quantity: 1, bulk: 0, price: 0, description: '' }

function InventoryTable({ characterId }) {
  const { data: items, error: loadError, reload } = useFetch(() => inventoryApi.list(characterId), [characterId])
  const [form, setForm] = useState(EMPTY_ITEM)
  const [editingId, setEditingId] = useState(null)
  const [error, setError] = useState(null)

  const update = (field, isNumber = false) => (event) => {
    const value = isNumber ? Number(event.target.value) : event.target.value
    setForm({ ...form, [field]: value })
  }

  const resetForm = () => {
    setForm(EMPTY_ITEM)
    setEditingId(null)
    setError(null)
  }

  const handleSubmit = async (event) => {
    event.preventDefault()
    try {
      if (editingId) {
        await inventoryApi.update(characterId, editingId, form)
      } else {
        await inventoryApi.create(characterId, form)
      }
      resetForm()
      reload()
    } catch (err) {
      setError(err)
    }
  }

  const startEdit = (item) => {
    setEditingId(item.id)
    setForm({ ...item, description: item.description ?? '' })
  }

  const handleDelete = async (item) => {
    if (!window.confirm(`Remove ${item.name}?`)) return
    try {
      await inventoryApi.remove(characterId, item.id)
      reload()
    } catch (err) {
      setError(err)
    }
  }

  const totalBulk = items?.reduce((sum, item) => sum + item.bulk * item.quantity, 0) ?? 0
  const totalValue = items?.reduce((sum, item) => sum + item.price * item.quantity, 0) ?? 0

  return (
    <div className="card">
      <h2>Inventory</h2>
      <ErrorMessage error={loadError} />

      <table>
        <thead>
          <tr>
            <th>Item</th>
            <th className="number">Qty</th>
            <th className="number">Bulk</th>
            <th className="number">Price (gp)</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {items?.map((item) => (
            <tr key={item.id}>
              <td>
                <strong>{item.name}</strong>
                {item.description && <div className="muted small">{item.description}</div>}
              </td>
              <td className="number">{item.quantity}</td>
              <td className="number">{formatBulk(item.bulk)}</td>
              <td className="number">{item.price}</td>
              <td className="number">
                <button className="small secondary" onClick={() => startEdit(item)}>Edit</button>{' '}
                <button className="small danger" onClick={() => handleDelete(item)}>Delete</button>
              </td>
            </tr>
          ))}
          {items?.length === 0 && (
            <tr>
              <td colSpan="5" className="muted">The backpack is empty.</td>
            </tr>
          )}
        </tbody>
        <tfoot>
          <tr>
            <td>Total</td>
            <td></td>
            <td className="number">{Math.round(totalBulk * 10) / 10}</td>
            <td className="number">{Math.round(totalValue * 100) / 100}</td>
            <td></td>
          </tr>
        </tfoot>
      </table>

      <form className="form inventory-form" onSubmit={handleSubmit}>
        <h3>{editingId ? 'Edit item' : 'Add item'}</h3>
        <div className="form-grid">
          <label>
            Name
            <input value={form.name} onChange={update('name')} required />
          </label>
          <label>
            Quantity
            <input type="number" min="1" value={form.quantity} onChange={update('quantity', true)} />
          </label>
          <label>
            Bulk <span className="muted">(0.1 = L)</span>
            <input type="number" min="0" step="0.1" value={form.bulk} onChange={update('bulk', true)} />
          </label>
          <label>
            Price (gp)
            <input type="number" min="0" step="0.01" value={form.price} onChange={update('price', true)} />
          </label>
        </div>
        <label>
          Description
          <input value={form.description} onChange={update('description')} />
        </label>
        <ErrorMessage error={error} />
        <div className="row">
          <button type="submit">{editingId ? 'Save item' : 'Add item'}</button>
          {editingId && (
            <button type="button" className="secondary" onClick={resetForm}>
              Cancel
            </button>
          )}
        </div>
      </form>
    </div>
  )
}

export default InventoryTable
