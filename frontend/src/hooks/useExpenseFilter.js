import { useSearchParams } from 'react-router-dom'

/**
 * Expense filter (from, to, categoryId) stored in the URL query string,
 * so the list and the charts page share it and it survives a reload.
 */
export function useExpenseFilter() {
  const [params, setParams] = useSearchParams()

  const filter = {
    from: params.get('from') ?? '',
    to: params.get('to') ?? '',
    categoryId: params.get('categoryId') ?? '',
  }

  const setFilter = (changes) => {
    const next = { ...filter, ...changes }
    const cleaned = Object.fromEntries(Object.entries(next).filter(([, value]) => value !== ''))
    setParams(cleaned, { replace: true })
  }

  return [filter, setFilter, params.toString()]
}
