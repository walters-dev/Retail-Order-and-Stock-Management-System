import { useState } from 'react'

const actionsByStatus = {
  Pending: [
    { action: 'confirm', label: 'Confirm order', roles: ['Admin'] },
    { action: 'cancel', label: 'Cancel order', roles: ['Admin', 'Staff'] },
  ],
  Confirmed: [
    { action: 'complete', label: 'Complete order', roles: ['Admin'] },
  ],
  Completed: [],
  Cancelled: [],
}

export default function OrderActions({ order, onStatusChanged, role = 'Admin' }) {
  const [isBusy, setIsBusy] = useState(false)
  const token = localStorage.getItem('TOKEN_KEY');
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')

  const actions = (actionsByStatus[order.status] ?? []).filter((a) =>
    a.roles.includes(role)
  )

  async function handleAction(action) {
    if (isBusy) return

    // Cancelling can't be undone, so ask first.
    if (action === 'cancel' && !window.confirm('Cancel this order?')) return

    setIsBusy(true)
    setError('')
    setMessage('')

    try {
      const response = await fetch(`/api/orders/${order.orderId}/${action}`, {
        method: 'PUT',
      })
      const result = await response.json().catch(() => null)

      if (!response.ok) {
        setError(result?.message ?? 'The order could not be updated. Please try again.')
        return
      }

      setMessage(result.message)
      onStatusChanged(result.order)
    } catch (err) {
      console.error(err)
      setError('Unable to reach the server. Please try again.')
    } finally {
      setIsBusy(false)
    }
  }

  return (
    <div className="order-actions">
      {actions.map(({ action, label }) => (
        <button
          key={action}
          type="button"
          onClick={() => handleAction(action)}
          disabled={isBusy}
        >
          {isBusy ? 'Working…' : label}
        </button>
      ))}

      {message && <p role="status">{message}</p>}
      {error && <p role="alert">{error}</p>}
    </div>
  )
}