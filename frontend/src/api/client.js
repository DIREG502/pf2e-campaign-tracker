const BASE_URL = import.meta.env.VITE_API_URL ?? ''
const TOKEN_KEY = 'pf2e-tracker-token'

export function getToken() {
  return localStorage.getItem(TOKEN_KEY)
}

export function setToken(token) {
  if (token) {
    localStorage.setItem(TOKEN_KEY, token)
  } else {
    localStorage.removeItem(TOKEN_KEY)
  }
}

/** Error thrown for non-2xx responses. Carries the ProblemDetails body if there is one. */
export class ApiError extends Error {
  constructor(status, problem) {
    super(problem?.detail || problem?.title || `Request failed with status ${status}`)
    this.status = status
    this.problem = problem
  }

  /** Field validation errors from ASP.NET Core, e.g. { Name: ['The Name field is required.'] } */
  get fieldErrors() {
    return this.problem?.errors ?? {}
  }
}

function buildUrl(path, query) {
  const url = new URL(BASE_URL + path, window.location.origin)
  if (query) {
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        url.searchParams.set(key, value)
      }
    })
  }
  return url
}

async function request(method, path, { body, query, raw = false } = {}) {
  const headers = {}
  const token = getToken()
  if (token) headers.Authorization = `Bearer ${token}`
  if (body !== undefined) headers['Content-Type'] = 'application/json'

  const response = await fetch(buildUrl(path, query), {
    method,
    headers,
    body: body !== undefined ? JSON.stringify(body) : undefined,
  })

  if (response.status === 401 && token) {
    // Token expired or invalid: let the auth context log the user out.
    window.dispatchEvent(new Event('auth:unauthorized'))
  }

  if (!response.ok) {
    let problem = null
    try {
      problem = await response.json()
    } catch {
      // Body was empty or not JSON.
    }
    throw new ApiError(response.status, problem)
  }

  if (raw) return response
  if (response.status === 204) return null
  return response.json()
}

export const api = {
  get: (path, query) => request('GET', path, { query }),
  post: (path, body) => request('POST', path, { body }),
  put: (path, body) => request('PUT', path, { body }),
  delete: (path) => request('DELETE', path),
  download: (path, query) => request('GET', path, { query, raw: true }),
}
