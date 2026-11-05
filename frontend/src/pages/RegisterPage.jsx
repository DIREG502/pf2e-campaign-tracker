import { useState } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import ErrorMessage from '../components/ErrorMessage'
import { useAuth } from '../hooks/useAuth'

function RegisterPage() {
  const { user, register } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState({ email: '', displayName: '', password: '' })
  const [error, setError] = useState(null)
  const [submitting, setSubmitting] = useState(false)

  if (user) return <Navigate to="/" replace />

  const update = (field) => (event) => setForm({ ...form, [field]: event.target.value })

  const handleSubmit = async (event) => {
    event.preventDefault()
    setSubmitting(true)
    setError(null)
    try {
      await register(form.email, form.displayName, form.password)
      navigate('/', { replace: true })
    } catch (err) {
      setError(err)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="auth-card card">
      <h1>Create account</h1>
      <form onSubmit={handleSubmit} className="form">
        <label>
          Email
          <input type="email" value={form.email} onChange={update('email')} required autoFocus />
        </label>
        <label>
          Display name
          <input value={form.displayName} onChange={update('displayName')} required minLength={2} />
        </label>
        <label>
          Password <span className="muted">(at least 8 characters)</span>
          <input type="password" value={form.password} onChange={update('password')} required minLength={8} />
        </label>
        <ErrorMessage error={error} />
        <button type="submit" disabled={submitting}>
          {submitting ? 'Creating account…' : 'Register'}
        </button>
      </form>
      <p className="muted">
        Already registered? <Link to="/login">Log in</Link>
      </p>
    </div>
  )
}

export default RegisterPage
