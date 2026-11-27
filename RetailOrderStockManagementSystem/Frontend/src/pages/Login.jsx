import { useState } from 'react'
// import './Login.css'
// import {UserTypes} from'../types/User.js'
import {useAuth} from '../auth/authContext.jsx'
//import './Register.jsx'




function Login(){
// function TextInput() {
//    const [text, setText] = useState('');   
//};
const {loginAttempt} = useAuth();
const [userName, setUserName] = useState('');
const [password, setPassword] = useState('');

const handleSubmit = (event) => {
        event?.preventDefault();
         loginAttempt({userName, password});
    }
    return(
        // <section>
        <div>
        {/* <section id= "center"> 
            <h1>Login</h1>
        </section> */}

        <form onSubmit={handleSubmit}> 
            <h1>Login</h1>
            <input
                type ="text"
                id="userName"
                name="userName"
                placeholder="Enter Username"
                value={userName}
                onChange={c => setUserName(c.target.value)}//refreshes the value inputted
                maxLength="20"
                required/>
            
            <input
                type ="password"
                id="password"
                name="password"
                placeholder="Enter Password"
                value={password}
                onChange={c => setPassword(c.target.value)}
                maxLength="30"
                required/>
            <button type="submit">Login</button>
            
            <button>Password reset</button>
         </form> 
        </div>
        // </section>
    )
    
    
}
export default Login;