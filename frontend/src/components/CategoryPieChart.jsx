import { ArcElement, Chart as ChartJS, Legend, Tooltip } from 'chart.js'
import { Pie } from 'react-chartjs-2'
import { formatMoney } from '../utils/format'

ChartJS.register(ArcElement, Tooltip, Legend)

/**
 * Pie chart of totals per category. Each slice uses the category's own color,
 * so a category keeps its color no matter how the filter changes the list.
 * The table next to it shows the exact numbers (and doesn't rely on color).
 */
function CategoryPieChart({ byCategory, total }) {
  const data = {
    labels: byCategory.map((c) => c.name),
    datasets: [
      {
        data: byCategory.map((c) => c.total),
        backgroundColor: byCategory.map((c) => c.color),
        // A thin white gap between slices keeps similar colors apart.
        borderColor: '#ffffff',
        borderWidth: 2,
        hoverOffset: 6,
      },
    ],
  }

  const options = {
    maintainAspectRatio: false,
    plugins: {
      legend: { position: 'bottom', labels: { usePointStyle: true, padding: 14 } },
      tooltip: {
        callbacks: {
          label: (context) => {
            const share = total > 0 ? Math.round((context.parsed / total) * 100) : 0
            return ` ${formatMoney(context.parsed)} (${share}%)`
          },
        },
      },
    },
  }

  return (
    <div className="pie-layout">
      <div className="chart-box">
        <Pie data={data} options={options} aria-label="Expenses by category" role="img" />
      </div>
      <table className="compact">
        <thead>
          <tr>
            <th>Category</th>
            <th className="number">Total</th>
            <th className="number">Share</th>
          </tr>
        </thead>
        <tbody>
          {byCategory.map((c) => (
            <tr key={c.categoryId}>
              <td>
                <span className="swatch" style={{ background: c.color }} />
                {c.name}
              </td>
              <td className="number">{formatMoney(c.total)}</td>
              <td className="number">{total > 0 ? Math.round((c.total / total) * 100) : 0}%</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

export default CategoryPieChart
