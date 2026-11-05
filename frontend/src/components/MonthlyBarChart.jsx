import { BarElement, CategoryScale, Chart as ChartJS, LinearScale, Tooltip } from 'chart.js'
import { Bar } from 'react-chartjs-2'
import { formatMoney } from '../utils/format'

ChartJS.register(BarElement, CategoryScale, LinearScale, Tooltip)

function monthLabel(month) {
  const [year, monthNumber] = month.split('-').map(Number)
  return new Date(year, monthNumber - 1, 1).toLocaleDateString(undefined, { month: 'short', year: 'numeric' })
}

/** One series (total per month), so no legend: the section title names it. */
function MonthlyBarChart({ byMonth }) {
  const data = {
    labels: byMonth.map((m) => monthLabel(m.month)),
    datasets: [
      {
        label: 'Total',
        data: byMonth.map((m) => m.total),
        backgroundColor: '#2a78d6',
        hoverBackgroundColor: '#1c5cab',
        borderRadius: 4,
        borderSkipped: 'start',
        maxBarThickness: 36,
      },
    ],
  }

  const options = {
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false },
      tooltip: {
        callbacks: { label: (context) => ` ${formatMoney(context.parsed.y)}` },
      },
    },
    scales: {
      x: { grid: { display: false } },
      y: {
        beginAtZero: true,
        border: { display: false },
        grid: { color: '#ece8e1' },
        ticks: { callback: (value) => formatMoney(value) },
      },
    },
  }

  return (
    <div className="chart-box">
      <Bar data={data} options={options} aria-label="Expenses per month" role="img" />
    </div>
  )
}

export default MonthlyBarChart
