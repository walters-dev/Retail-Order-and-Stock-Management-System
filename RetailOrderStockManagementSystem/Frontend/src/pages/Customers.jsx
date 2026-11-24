import { useEffect, useState } from 'react'
import './Customers.css'

const blankCustomer = {
  fullName: '',
  mobileNumber: '',
  email: '',
  isActive: true,
}

export default function Customers ()  {
  const [customers, setCustomers] = useState([])
  const [isLoading, setIsLoading] = useState(true)
  const [loadError, setLoadError] = useState('')
  const [searchText, setSearchText] = useState('')
  const [selectedCustomer, setSelectedCustomer] = useState(null)
  const [formError, setFormError] = useState('')
  /* isSaving tracks a contact-details save. 
  isDeactivating tracks a status change. 
  isBusy becomes true when either operation is running, 
  using the same || operator as C#. */
  const [isSaving, setIsSaving] = useState(false)
  const [isUpdatingStatus, setIsUpdatingStatus] = useState(false)
  const isBusy = isSaving || isUpdatingStatus

  // Load customers when this component appears. Cancel the request during cleanup.
  useEffect(() => {
    const controller = new AbortController()

    async function loadCustomers() {
      try {
        const response = await fetch(
          'https://localhost:7041/api/customers',
          { signal: controller.signal }
        )

        if (!response.ok) {
          throw new Error(`Customer request failed (${response.status})`)
        }

        const data = await response.json()

        if (controller.signal.aborted) return

        setCustomers(
          data.map((customer) => ({
            ...customer,
            id: customer.customerId,
          }))
        )
      } catch (error) {
        if (controller.signal.aborted) return

        console.error(error)
        setLoadError('Customers could not be loaded. Refresh to try again.')
      } finally {
        if (!controller.signal.aborted) {
          setIsLoading(false)
        }
      }
    }

    loadCustomers()

    return () => controller.abort()
  }, [])

  const searchTerm = searchText.trim().toLowerCase()
  const matchingCustomers = customers.filter((customer) =>
    customer.fullName.toLowerCase().includes(searchTerm)
  )

  function openForm(customer) {
    setSelectedCustomer(customer)
    setFormError('')
  }

  // Send new customers with POST and existing customer changes with PUT.
  async function handleSave(event) {
    event.preventDefault()

    if (isBusy) return /* This prevents a save from starting while deactivation is running.*/

    const formData = new FormData(event.currentTarget)
    const customerId = selectedCustomer.id
    const isEditing = customerId !== undefined

    const request = {
      fullName: formData.get('fullName').trim(),
      mobileNumber: formData.get('mobileNumber').trim(),
      email: formData.get('email').trim() || null,
    }

    setFormError('')

    if (!request.fullName || !request.mobileNumber) {
      setFormError('Enter a full name and mobile number.')
      return
    }

    if (isEditing) {
      request.loyaltyCustomer = formData.has('loyaltyCustomer')
    }

    const url = isEditing
      ? `https://localhost:7041/api/customers/${customerId}`
      : 'https://localhost:7041/api/customers'

    setIsSaving(true)
    try {
      const response = await fetch(url, {
        method: isEditing ? 'PUT' : 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(request),
      })

      if (!response.ok) {
        const problem = await response.json().catch(() => null)
        const validationErrors = Object.values(problem?.errors ?? {}).flat()

        setFormError(
          validationErrors.join(' ') ||
          problem?.message ||
          'Customer could not be saved. Please try again.'
        )

        return
      }

      if (isEditing) {
        // PUT returns 204 with an empty body.
        setCustomers((currentCustomers) =>
          currentCustomers.map((customer) =>
            customer.id === customerId
              ? { ...customer, ...request }
              : customer
          )
        )
      } else {
        // POST returns the saved customer, including its database ID.
        const createdCustomer = await response.json()

        setCustomers((currentCustomers) => [
          ...currentCustomers,
          {
            ...createdCustomer,
            id: createdCustomer.customerId,
          },
        ])
      }

      setSelectedCustomer(null)
    } catch (error) {
      console.error(error)
      setFormError('Unable to save the customer. Please try again.')
    } finally {
      setIsSaving(false)
    }
  }

  async function handleStatusChange() {
  if (isBusy) return

  const customerId = selectedCustomer.id
  const nextIsActive = !selectedCustomer.isActive

  setFormError('')
  setIsUpdatingStatus(true)

  try {
    const response = await fetch(
      `https://localhost:7041/api/customers/${customerId}/isActive?isActive=${nextIsActive}`,
      { method: 'PUT' }
    )

    if (!response.ok) {
      setFormError('Customer status could not be updated. Please try again.')
      return
    }

    setCustomers((currentCustomers) =>
      currentCustomers.map((customer) =>
        customer.id === customerId
          ? { ...customer, isActive: nextIsActive }
          : customer
      )
    )

    setSelectedCustomer((currentCustomer) => ({
      ...currentCustomer,
      isActive: nextIsActive,
    }))
  } catch (error) {
    console.error(error)
    setFormError('Unable to update customer status. Please try again.')
  } finally {
    setIsUpdatingStatus(false)
  }
}

  // These checks belong inside Customers, before its main page return.
  if (isLoading) {
    return (
      <main>
        <h1>Customers</h1>
        <p role="status">Loading customers…</p>
      </main>
    )
  }

  if (loadError) {
    return (
      <main>
        <h1>Customers</h1>
        <p role="alert">{loadError}</p>
      </main>
    )
  }

  return (
    <main>
      <h1>Customers</h1>

      <div className="customers-layout">
        <section className="customers-list">
          <label htmlFor="customer-search">Search by name</label>
          <input
            id="customer-search"
            type="search"
            value={searchText}
            onChange={(event) => setSearchText(event.target.value)}
            disabled={isBusy}
          />
          <button
            type="button"
            onClick={() => openForm(blankCustomer)}
            disabled={isBusy}
          >
            + Add Customer
          </button>

          <table>
            <thead>
              <tr>
                <th scope="col">Name</th>
                <th scope="col">Mobile</th>
                <th scope="col">Status</th>
                <th scope="col">Actions</th>
              </tr>
            </thead>

            <tbody>
              {matchingCustomers.length === 0 && (
                <tr>
                  <td colSpan={4}>
                    {customers.length === 0
                      ? 'No customers yet. Select + Add Customer to create one.'
                      : 'No customers match your search.'}
                  </td>
                </tr>
              )}

              {matchingCustomers.map((customer) => (
                <tr key={customer.id}>
                  <td>{customer.fullName}</td>
                  <td>{customer.mobileNumber}</td>
                  <td>
                    <span
                      className={customer.isActive ? 'status-active' : 'status-inactive'}
                    >
                      {customer.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </td>
                  <td>
                    <button
                      type="button"
                      onClick={() => openForm(customer)}
                      disabled={isBusy}
                    >
                      Edit
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </section>

        {selectedCustomer && (
          <section className="customer-editor">
            <h2>{selectedCustomer.id ? 'Edit Customer' : 'Add Customer'}</h2>

            <form
              /*aria-busy tells screen readers that the form is processing an action.*/
              key={selectedCustomer.id ?? 'new'}
              onSubmit={handleSave}
              aria-busy={isBusy}
            >
              <label>
                Full name
                <input
                  name="fullName"
                  defaultValue={selectedCustomer.fullName}
                  required
                  disabled={isBusy}
                />
              </label>

              <label>
                Mobile number
                <input
                  name="mobileNumber"
                  type="tel"
                  defaultValue={selectedCustomer.mobileNumber}
                  required
                  disabled={isBusy}
                />
              </label>

              <label>
                Email (optional)
                <input
                  name="email"
                  type="email"
                  defaultValue={selectedCustomer.email ?? ''}
                  disabled={isBusy}
                />
              </label>

              {selectedCustomer.id && (
                <label>
                  <input
                    name="loyaltyCustomer"
                    type="checkbox"
                    defaultChecked={selectedCustomer.loyaltyCustomer}
                    disabled={isBusy}
                  />
                  Loyalty customer
                </label>
              )}

              {formError && <p role="alert">{formError}</p>}

              <div>
              {selectedCustomer.id && (
                <button
                  type="button"
                  onClick={handleStatusChange}
                  disabled={isBusy}
                >
                  {isUpdatingStatus
                    ? 'Updating status…'
                    : selectedCustomer.isActive
                      ? 'Deactivate customer'
                      : 'Reactivate customer'}
                </button>
              )}

              <button
                type="button"
                onClick={() => setSelectedCustomer(null)}
                disabled={isBusy}
              >
                Cancel
              </button>

              <button type="submit" disabled={isBusy}>
                {isSaving ? 'Saving…' : 'Save'}
              </button>
            </div>
            </form>
          </section>
        )}
      </div>
    </main>
  )
}

