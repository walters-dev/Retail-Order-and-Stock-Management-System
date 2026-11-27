import { useState } from 'react';

const ROLE_KEY = "role";
const TOKEN_KEY = "token"
export default function LogoutButton() {
  const [role, setRole] = useState(JSON.parse(localStorage.getItem(ROLE_KEY), "{}"));
   const [token, setToken] = useState(localStorage.getItem(TOKEN_KEY) || "");
  //  setToken(localStorage.getItem(TOKEN_KEY));
const logout = () => {
            setToken("");
            setRole(null);
            localStorage.removeItem(ROLE_KEY);
            localStorage.removeItem(TOKEN_KEY);
        }

  
 // Nobody is logged in, so there is nothing to log out of.
  // if (!token) return null;

  return (
    <button type="buton" className="sidebar-logout" onClick={() => logout()}>
        Log out
    </button>
  );
}