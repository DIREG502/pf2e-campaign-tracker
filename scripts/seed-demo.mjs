// Fills a running backend with demo data through the public API.
//
//   node scripts/seed-demo.mjs [apiBaseUrl]
//
// Creates (or reuses) the user demo@example.com / demo-password-123.
// These are throwaway local credentials for trying the app, not a real account.

const API = (process.argv[2] ?? 'http://localhost:5080').replace(/\/$/, '')
const DEMO_USER = { email: 'demo@example.com', displayName: 'Demo GM', password: 'demo-password-123' }

let token = null

async function call(method, path, body) {
  const response = await fetch(`${API}${path}`, {
    method,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    body: body ? JSON.stringify(body) : undefined,
  })

  if (!response.ok) {
    const text = await response.text()
    throw new Error(`${method} ${path} failed with ${response.status}: ${text}`)
  }
  return response.status === 204 ? null : response.json()
}

async function login() {
  try {
    return await call('POST', '/api/auth/register', DEMO_USER)
  } catch {
    return call('POST', '/api/auth/login', { email: DEMO_USER.email, password: DEMO_USER.password })
  }
}

function isoDate(monthsAgo, day) {
  const now = new Date()
  const date = new Date(now.getFullYear(), now.getMonth() - monthsAgo, day)
  return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`
}

async function seedCampaign() {
  const campaign = await call('POST', '/api/campaigns', {
    name: 'The Sunken Crown',
    description: 'A homebrew dungeon crawl beneath the drowned city of Velmora.',
  })

  const characters = [
    {
      name: 'Mirela Voss', ancestry: 'Human', class: 'Wizard', level: 4,
      currentHp: 38, maxHp: 46, armorClass: 18,
      strength: 0, dexterity: 2, constitution: 1, intelligence: 4, wisdom: 1, charisma: 0,
      notes: 'Arcane school of battle magic. Owes the guild 40 gp.',
    },
    {
      name: 'Brannoc Ironvein', ancestry: 'Dwarf', class: 'Fighter', level: 4,
      currentHp: 64, maxHp: 64, armorClass: 22,
      strength: 4, dexterity: 1, constitution: 3, intelligence: 0, wisdom: 1, charisma: -1,
      notes: 'Shield block every round. Hates boats.',
    },
    {
      name: 'Sefa Lightfoot', ancestry: 'Halfling', class: 'Rogue', level: 4,
      currentHp: 15, maxHp: 52, armorClass: 21,
      strength: 0, dexterity: 4, constitution: 1, intelligence: 2, wisdom: 0, charisma: 2,
    },
    {
      name: 'Ilsari Moonwhisper', ancestry: 'Elf', class: 'Cleric', level: 4,
      currentHp: 41, maxHp: 44, armorClass: 19,
      strength: 1, dexterity: 1, constitution: 1, intelligence: 0, wisdom: 4, charisma: 2,
    },
  ]

  const created = []
  for (const character of characters) {
    created.push(await call('POST', '/api/characters', { ...character, campaignId: campaign.id }))
  }

  const items = [
    { name: 'Staff of Embers', quantity: 1, bulk: 1, price: 90, description: 'Casts produce flame once per day.' },
    { name: 'Minor Healing Potion', quantity: 3, bulk: 0.1, price: 4 },
    { name: 'Scroll of Fly', quantity: 1, bulk: 0.1, price: 30 },
    { name: 'Spellbook', quantity: 1, bulk: 1, price: 10, description: 'Bound in eel leather.' },
    { name: 'Rations (1 week)', quantity: 2, bulk: 0.1, price: 0.4 },
    { name: 'Silver Dagger', quantity: 1, bulk: 0.1, price: 12 },
  ]
  for (const item of items) {
    await call('POST', `/api/characters/${created[0].id}/items`, item)
  }
  await call('POST', `/api/characters/${created[1].id}/items`, { name: 'Steel Shield', quantity: 1, bulk: 1, price: 2 })

  const notes = [
    {
      title: 'Session 1: Into the flooded gate',
      sessionDate: isoDate(2, 4),
      tags: ['combat', 'exploration'],
      content: '## Summary\nThe party forced the **flooded gate** and fought two drowned sentries.\n\n- Brannoc nearly drowned\n- Found a map fragment',
    },
    {
      title: 'Session 2: The eel market',
      sessionDate: isoDate(1, 12),
      tags: ['roleplay', 'loot'],
      content: 'Haggled with the eel merchants. Mirela bought a *Scroll of Fly*.\n\n> "Nothing in Velmora stays dry for long."',
    },
    {
      title: 'Session 3: Crown chamber',
      sessionDate: isoDate(0, 2),
      tags: ['combat', 'boss'],
      content: 'Reached the crown chamber. Combat against the **Tide Warden** is still running.',
    },
  ]
  for (const note of notes) {
    await call('POST', `/api/campaigns/${campaign.id}/notes`, note)
  }

  const encounter = await call('POST', '/api/encounters', { name: 'Tide Warden', campaignId: campaign.id })
  const initiatives = [17, 12, 21, 15]
  for (let i = 0; i < created.length; i++) {
    await call('POST', `/api/encounters/${encounter.id}/combatants/from-character`, {
      characterId: created[i].id,
      initiative: initiatives[i],
    })
  }
  await call('POST', `/api/encounters/${encounter.id}/combatants`, {
    name: 'Tide Warden', initiative: 21, currentHp: 72, maxHp: 110, conditions: ['frightened 1'], isPlayerCharacter: false,
  })
  await call('POST', `/api/encounters/${encounter.id}/combatants`, {
    name: 'Drowned Sentry', initiative: 9, currentHp: 18, maxHp: 30, conditions: ['off-guard'], isPlayerCharacter: false,
  })
  // Round 2, third turn.
  for (let i = 0; i < 9; i++) {
    await call('POST', `/api/encounters/${encounter.id}/next-turn`)
  }

  return { campaign, characters: created, encounter }
}

async function seedExpenses() {
  const categories = await call('GET', '/api/categories')
  const byName = Object.fromEntries(categories.map((c) => [c.name, c.id]))

  const monthly = [
    ['Rent', 'Monthly rent', 720, 1],
    ['Food', 'Groceries', 64.3, 6],
    ['Food', 'Groceries', 58.9, 20],
    ['Transport', 'Train ticket', 49, 3],
    ['Entertainment', 'Cinema', 24, 14],
    ['Games & Hobbies', 'Pizza for game night', 38.5, 18],
  ]
  const extras = [
    [0, 'Games & Hobbies', 'PF2e Player Core', 59.99, 8],
    [1, 'Games & Hobbies', 'Dice set', 17.5, 10],
    [2, 'Other', 'Birthday present', 45, 22],
    [3, 'Games & Hobbies', 'Battle map and minis', 72.4, 5],
    [4, 'Transport', 'Bike repair', 85, 11],
    [5, 'Entertainment', 'Concert ticket', 65, 16],
  ]

  for (let monthsAgo = 0; monthsAgo < 6; monthsAgo++) {
    for (const [category, description, amount, day] of monthly) {
      // Vary amounts a little so the bar chart is not flat.
      const variation = category === 'Rent' ? 0 : ((monthsAgo * 7) % 5) * 1.75
      await call('POST', '/api/expenses', {
        amount: Math.round((amount + variation) * 100) / 100,
        date: isoDate(monthsAgo, day),
        description,
        categoryId: byName[category],
      })
    }
  }
  for (const [monthsAgo, category, description, amount, day] of extras) {
    await call('POST', '/api/expenses', { amount, date: isoDate(monthsAgo, day), description, categoryId: byName[category] })
  }
}

const auth = await login()
token = auth.token

const existing = await call('GET', '/api/campaigns')
if (existing.length > 0) {
  console.log('Demo user already has data, nothing to do.')
} else {
  const { characters, encounter } = await seedCampaign()
  await seedExpenses()
  console.log(`Seeded ${characters.length} characters, encounter #${encounter.id} and 42 expenses.`)
}

console.log(`Log in with ${DEMO_USER.email} / ${DEMO_USER.password}`)
