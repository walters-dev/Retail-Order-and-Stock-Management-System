import { useState } from 'react'
import styles from './Register.module.css'

const types = [
    'Manager',
    'Admin',
    'Staff'
]

export default function Registry() {
    const token = localStorage.getItem('token');
    const [userName, setUserName] = useState('');
    const [password, setPassword] = useState('');
    const [email, setEmail] = useState('');
    const [selectedType, setSelectedType] = useState(types)
    const handleSubmit = (event) => {
        event?.preventDefault();
        registerAttempt({ userName, password, email,  selectedType});
    }
    const registerAttempt = async (data) => {
        try {
            const response = await fetch('/api/Auth', {
                method: "Post",
                // signal: controller.signal,
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`,
                    
                },
                  body: JSON.stringify(data)//serializer
            });
            if (!response.ok) {
                throw new Error('Reports request failed: ${response.status}');
            }
            //setResult();
            //const data = await response.json();
        } catch (error) {
            console.log(error);
        }
    }

    return (
        <>
            <h1>Registration</h1>
            <div className={styles.layout}>
            <section className={styles.editor}>
            <form onSubmit={handleSubmit} className={styles.loginform}>
                {/* <h1 className={styles.title}>Login</h1> */}
                <input
                    type="text"
                    id="userName"
                    name="userName"
                    placeholder="Enter Username"
                    value={userName}
                    onChange={c => setUserName(c.target.value)}//refreshes the value inputted
                    maxLength="20"
                    required
                    className={styles.userinput} />

                <input
                    type="password"
                    id="password"
                    name="password"
                    placeholder="Enter Password"
                    value={password}
                    onChange={c => setPassword(c.target.value)}
                    maxLength="30"
                    className={styles.userinput}
                    required />

                <input
                    type="text"
                    id="email"
                    name="email"
                    placeholder="Enter Email"
                    value={email}
                    onChange={c => setEmail(c.target.value)}
                    maxLength="30"
                    className={styles.userinput}
                    required />
<div className={styles.buttonlayout}>
                {types.map((type) => (
                    <button
                        key={type}
                        type="button"
                        className={styles.buttons}
                        aria-pressed={selectedType === type}
                        onClick={() => setSelectedType(type)}
                        required
                    >
                        {type}
                    </button>
                ))}
                </div>
                <div className={styles.buttonlayout}>
                <button type="submit" className={styles.buttons}>Login</button>
           </div>
            </form>
            </section>
            </div>
        </>
    )
}