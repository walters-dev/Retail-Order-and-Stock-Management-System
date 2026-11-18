import { useState } from 'react'
import './Customers.css'

const sampleCustomers = [
  { id: 1, fullName: 'Rais Abrahams', mobileNumber: '082 123 4567', email: 'thabo@example.com', isActive: true },
  { id: 2, fullName: 'Ayesha Jacobs', mobileNumber: '083 234 5678', email: '', isActive: true },
  { id: 3, fullName: 'Naledi Khumalo', mobileNumber: '072 567 8901', email: '', isActive: false },
]

const blankCustomer = {
  fullName: '',
  mobileNumber: '',
  email: '',
  isActive: true,
}

export const Customers = () => {
  const [customers, setCustomers] = useState(sampleCustomers)
  const [searchText, setSearchText] = useState('')
  const [selectedCustomer, setSelectedCustomer] = useState(null)
  const [formError, setFormError] = useState('')

  const searchTerm = searchText.trim().toLowerCase()
  const matchingCustomers = customers.filter((customer) =>
    customer.fullName.toLowerCase().includes(searchTerm)
  )

  function openForm(customer) {
    setSelectedCustomer(customer)
    setFormError('')
  }

  function handleSave(event) {
    event.preventDefault()

    const formData = new FormData(event.currentTarget)
    const fullName = formData.get('fullName').trim()
    const mobileNumber = formData.get('mobileNumber').trim()

    if (!fullName || !mobileNumber) {
      setFormError('Enter a full name and mobile number.')
      return
    }

    const savedCustomer = {
      id: selectedCustomer.id ?? crypto.randomUUID(),
      fullName,
      mobileNumber,
      email: formData.get('email').trim(),
      isActive: formData.has('isActive'),
    }

    setCustomers((currentCustomers) => {
      if (selectedCustomer.id === undefined) {
        return [...currentCustomers, savedCustomer]
      }

      return currentCustomers.map((customer) =>
        customer.id === savedCustomer.id ? savedCustomer : customer
      )
    })

    setSelectedCustomer(null)
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
          />
          <button type="button" onClick={() => openForm(blankCustomer)}>
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
                  <td>{customer.isActive ? 'Active' : 'Inactive'}</td>
                  <td>
                    <button type="button" onClick={() => openForm(customer)}>
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

            <form key={selectedCustomer.id ?? 'new'} onSubmit={handleSave}>
              <label>
                Full name
                <input name="fullName" defaultValue={selectedCustomer.fullName} required />
              </label>

              <label>
                Mobile number
                <input name="mobileNumber" type="tel" defaultValue={selectedCustomer.mobileNumber} required />
              </label>

              <label>
                Email (optional)
                <input name="email" type="email" defaultValue={selectedCustomer.email} />
              </label>

              <label>
                <input name="isActive" type="checkbox" defaultChecked={selectedCustomer.isActive} />
                Active
              </label>

              {formError && <p role="alert">{formError}</p>}

              <div>
                <button type="button" onClick={() => setSelectedCustomer(null)}>
                  Cancel
                </button>
                <button type="submit">Save</button>
              </div>
            </form>
          </section>
        )}
      </div>
    </main>
  )
}