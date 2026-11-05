import { api } from './client'

export const campaignsApi = {
  list: () => api.get('/api/campaigns'),
  get: (id) => api.get(`/api/campaigns/${id}`),
  create: (data) => api.post('/api/campaigns', data),
  update: (id, data) => api.put(`/api/campaigns/${id}`, data),
  remove: (id) => api.delete(`/api/campaigns/${id}`),
}

export const charactersApi = {
  list: (campaignId) => api.get('/api/characters', { campaignId }),
  get: (id) => api.get(`/api/characters/${id}`),
  create: (data) => api.post('/api/characters', data),
  update: (id, data) => api.put(`/api/characters/${id}`, data),
  remove: (id) => api.delete(`/api/characters/${id}`),
}

export const inventoryApi = {
  list: (characterId) => api.get(`/api/characters/${characterId}/items`),
  create: (characterId, data) => api.post(`/api/characters/${characterId}/items`, data),
  update: (characterId, id, data) => api.put(`/api/characters/${characterId}/items/${id}`, data),
  remove: (characterId, id) => api.delete(`/api/characters/${characterId}/items/${id}`),
}

export const notesApi = {
  list: (campaignId, tag) => api.get(`/api/campaigns/${campaignId}/notes`, { tag }),
  create: (campaignId, data) => api.post(`/api/campaigns/${campaignId}/notes`, data),
  update: (campaignId, id, data) => api.put(`/api/campaigns/${campaignId}/notes/${id}`, data),
  remove: (campaignId, id) => api.delete(`/api/campaigns/${campaignId}/notes/${id}`),
}

export const encountersApi = {
  list: () => api.get('/api/encounters'),
  get: (id) => api.get(`/api/encounters/${id}`),
  create: (data) => api.post('/api/encounters', data),
  remove: (id) => api.delete(`/api/encounters/${id}`),
  addCombatant: (id, data) => api.post(`/api/encounters/${id}/combatants`, data),
  addCharacter: (id, data) => api.post(`/api/encounters/${id}/combatants/from-character`, data),
  updateCombatant: (id, combatantId, data) => api.put(`/api/encounters/${id}/combatants/${combatantId}`, data),
  removeCombatant: (id, combatantId) => api.delete(`/api/encounters/${id}/combatants/${combatantId}`),
  nextTurn: (id) => api.post(`/api/encounters/${id}/next-turn`),
  reset: (id) => api.post(`/api/encounters/${id}/reset`),
}
