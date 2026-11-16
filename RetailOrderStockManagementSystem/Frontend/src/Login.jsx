import { useState } from 'react'
import './Login.css'

function Login(){
// function TextInput() {
//    const [text, setText] = useState('');   
//};
const [userName, setUserName] = useState('');
const [password, setPassword] = useState('');
    return(
        <div>
        {/* <section id= "center"> 
            <h1>Login</h1>
        </section> */}

        <form id ="login">
            <h1>Login</h1>
            <input
                type ="text"
                id="userName"
                name="userName"
                placeholder="Enter Username"
                value={userName}
                onChange={c => setUserName(c.target.value)}
                maxLength="20"/>
            
            <input
                type ="password"
                id="password"
                name="password"
                placeholder="Enter Password"
                value={password}
                onChange={c => setPassword(c.target.value)}
                maxLength="30"/>
            <button>Login</button>
            <button>Password reset</button>
        </form>
        </div>
    )
}
export default Login;