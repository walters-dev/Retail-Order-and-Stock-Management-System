import { useState } from 'react'
import styles from './Login.module.css'
import {UserTypes} from'../types/User.js'
import {useAuth} from '../auth/authContext.jsx'
//import './Register.jsx'




function Login(){
// function TextInput() {
//    const [text, setText] = useState('');   
//};
const {loginAttempt} = useAuth();
const [userName, setUserName] = useState('');
const [password, setPassword] = useState('');
const [errorResponse, setErrorResponse] = useState('');
//logout();
const handleSubmit = async (event) => {
        event?.preventDefault();
        try{
            await loginAttempt({userName, password});
        }
        catch(error){
            setErrorResponse(error.message);
            console.log(error);
        }
         
    }
    return(
        // <section>
        <div className={styles.loginlayout}>
        {/* <section id= "center"> 
            <h1>Login</h1>
        </section> */}

        <form onSubmit={handleSubmit} className={styles.loginform}> 
            <h1 className={styles.title}>Login</h1>
            <input
                type ="text"
                id="userName"
                name="userName"
                placeholder="Enter Username"
                value={userName}
                onChange={c => setUserName(c.target.value)}//refreshes the value inputted
                maxLength="20"
                required
                className={styles.userinput}/>
            
            <input
                type ="password"
                id="password"
                name="password"
                placeholder="Enter Password"
                value={password}
                onChange={c => setPassword(c.target.value)}
                maxLength="30"
                className={styles.userinput}
                required/>
                {errorResponse && <p role="alert">{errorResponse}</p>}
            <button type="submit" className={styles.buttons}>Login</button>
            
            <button className={styles.buttons}>Password reset</button>
            
         </form> 
         
        </div>
        // </section>
    )
    
    
}
export default Login;