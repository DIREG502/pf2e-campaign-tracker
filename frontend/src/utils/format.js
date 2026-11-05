/** "+3", "0", "-1" */
export function formatModifier(value) {
  return value > 0 ? `+${value}` : String(value)
}

const currency = new Intl.NumberFormat(undefined, { style: 'currency', currency: 'EUR' })

export function formatMoney(value) {
  return currency.format(value)
}

const wholeCurrency = new Intl.NumberFormat(undefined, { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 })

/** Money without cents, for chart axes. */
export function formatMoneyShort(value) {
  return wholeCurrency.format(value)
}

/** PF2e bulk: 0.1 is shown as "L", 0 as "–". */
export function formatBulk(value) {
  if (value === 0) return '–'
  if (value < 1) return `${Math.round(value * 10)}L`
  return String(value)
}

/** Formats a "yyyy-MM-dd" date string in the user's locale without timezone shifts. */
export function formatDate(isoDate) {
  const [year, month, day] = isoDate.split('-').map(Number)
  return new Date(year, month - 1, day).toLocaleDateString()
}

export function todayIso() {
  const now = new Date()
  const month = String(now.getMonth() + 1).padStart(2, '0')
  const day = String(now.getDate()).padStart(2, '0')
  return `${now.getFullYear()}-${month}-${day}`
}
