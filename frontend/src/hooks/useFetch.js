import { useCallback, useEffect, useState } from 'react'

/**
 * Loads data with the given async function and re-runs it when `deps` change.
 * Returns { data, error, loading, reload, setData }.
 */
export function useFetch(fetcher, deps = []) {
  const [data, setData] = useState(null)
  const [error, setError] = useState(null)
  const [loading, setLoading] = useState(true)

  const load = useCallback(fetcher, deps)

  const reload = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      setData(await load())
    } catch (err) {
      setError(err)
    } finally {
      setLoading(false)
    }
  }, [load])

  useEffect(() => {
    reload()
  }, [reload])

  return { data, error, loading, reload, setData }
}
