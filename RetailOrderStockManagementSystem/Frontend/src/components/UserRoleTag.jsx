import { useAuth } from '../auth/authContext.jsx'
import '../pages/customers.css'
export default function UserRoleTag() {

    const role = localStorage.getItem('role');
    

  

    return (
        <div className='user-role-wrapper'>
        <label
            className="user-role-tag"
        >
            {`Logged in as ${role}`}
        </label>
        </div>
    )
}