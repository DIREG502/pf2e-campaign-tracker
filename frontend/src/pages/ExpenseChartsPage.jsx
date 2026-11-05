import { expensesApi } from '../api/expenses'
import CategoryPieChart from '../components/CategoryPieChart'
import ErrorMessage from '../components/ErrorMessage'
import ExpenseFilterBar from '../components/ExpenseFilterBar'
import { useExpenseFilter } from '../hooks/useExpenseFilter'
import { useFetch } from '../hooks/useFetch'
import { formatMoney } from '../utils/format'

function ExpenseChartsPage() {
  const [filter, setFilter] = useExpenseFilter()
  // The charts compare categories, so the category filter is not used here.
  const { data: summary, error } = useFetch(
    () => expensesApi.summary({ from: filter.from, to: filter.to }),
    [filter.from, filter.to],
  )

  const monthCount = summary?.byMonth.length ?? 0

  return (
    <>
      <div className="page-header">
        <h1>Expense charts</h1>
      </div>

      <ExpenseFilterBar filter={filter} onChange={setFilter} showCategory={false} />
      <ErrorMessage error={error} />

      {summary && summary.count === 0 && <p className="muted">No expenses in this range.</p>}

      {summary && summary.count > 0 && (
        <>
          <div className="stat-tiles">
            <div className="card stat-tile">
              <span className="label">Total</span>
              <span className="value">{formatMoney(summary.total)}</span>
            </div>
            <div className="card stat-tile">
              <span className="label">Expenses</span>
              <span className="value">{summary.count}</span>
            </div>
            <div className="card stat-tile">
              <span className="label">Avg. per month</span>
              <span className="value">{formatMoney(monthCount > 0 ? summary.total / monthCount : 0)}</span>
            </div>
          </div>

          <section className="card">
            <h2>By category</h2>
            <CategoryPieChart byCategory={summary.byCategory} total={summary.total} />
          </section>
        </>
      )}
    </>
  )
}

export default ExpenseChartsPage
