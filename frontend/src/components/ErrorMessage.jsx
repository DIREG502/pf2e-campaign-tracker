/** Shows an ApiError (or any Error) including field validation messages. */
function ErrorMessage({ error }) {
  if (!error) return null

  const fieldErrors = Object.entries(error.fieldErrors ?? {})

  return (
    <div className="error" role="alert">
      <strong>{error.problem?.title ?? error.message}</strong>
      {error.problem?.detail && <div>{error.problem.detail}</div>}
      {fieldErrors.length > 0 && (
        <ul>
          {fieldErrors.map(([field, messages]) => (
            <li key={field}>{messages.join(' ')}</li>
          ))}
        </ul>
      )}
    </div>
  )
}

export default ErrorMessage
