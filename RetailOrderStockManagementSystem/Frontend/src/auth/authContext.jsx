import { useState, useContext, createContext } from 'react'
import { useNavigate } from 'react-router-dom';
//import { defineConfig } from '../../vite.config'

const AuthContext = createContext();
const TOKEN_KEY = "token";
const ROLE_KEY = "role";

export const AuthProvider = ({children}) => {
    const [role, setRole] = useState(JSON.parse(localStorage.getItem(ROLE_KEY), "{}"));
    const [token, setToken] = useState(localStorage.getItem(TOKEN_KEY) || "");
    const navigation = useNavigate();
    const loginAttempt = async (data) => {
        try {
            const response = await fetch('/api/Auth/Login', {
                method: "POST",
                headers: { "Content-Type": "application/json" },//change cause of swagger??
                body: JSON.stringify(data)//serializer
            });
            if (!response.ok) {
                throw new Error('Login request failed: ${response.status}');
            }
            const result = await response.json();
            if (result) {
                setToken(result.token);
                setRole(result.role);
                localStorage.setItem(TOKEN_KEY, result.token);
                localStorage.setItem(ROLE_KEY, JSON.stringify(result.role));
                //router.navigate('/',{replace: true});//goes to index
                navigation('/');
                //window.location.reload();
            }
            console.log(result);
            //throw new Error(result.message);
        } catch (error) {
            console.log(error);
        }
        

    }
    // const logout = () => {
    //         setToken("");
    //         setRole(null);
    //         localStorage.removeItem(ROLE_KEY);
    //         localStorage.removeItem(TOKEN_KEY);
    //     }
    return (
        <AuthContext.Provider value={{token, role, loginAttempt}}>
            {children}
        </AuthContext.Provider>
    )
}



export const useAuth = ()=>{
    return useContext(AuthContext);
}
