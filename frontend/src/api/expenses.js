import { api } from './client'

export const categoriesApi = {
  list: () => api.get('/api/categories'),
  create: (data) => api.post('/api/categories', data),
  update: (id, data) => api.put(`/api/categories/${id}`, data),
  remove: (id) => api.delete(`/api/categories/${id}`),
}

export const expensesApi = {
  list: (filter) => api.get('/api/expenses', filter),
  summary: (filter) => api.get('/api/expenses/summary', filter),
  create: (data) => api.post('/api/expenses', data),
  update: (id, data) => api.put(`/api/expenses/${id}`, data),
  remove: (id) => api.delete(`/api/expenses/${id}`),

  /** Downloads the filtered expenses as CSV and lets the browser save the file. */
  async exportCsv(filter) {
    const response = await api.download('/api/expenses/export', filter)
    const blob = await response.blob()
    const disposition = response.headers.get('Content-Disposition') ?? ''
    const fileName = disposition.match(/filename=([^;]+)/)?.[1] ?? 'expenses.csv'

    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = fileName
    link.click()
    URL.revokeObjectURL(url)
  },
}
