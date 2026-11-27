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
    const [selectedType, setSelectedType] = useState(types);
    const [errorResponse, setErrorResponse] = useState('');
    const handleSubmit = (event) => {
        event?.preventDefault();
        registerAttempt({ userName, password, email, selectedType });
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
            if (response.status == 403) {
                throw new Error("You are not authorized to access register");
            } else if (!response.ok) {
                const body = await response.json().catch(() => null);//.catch(() => null) protects it from non json bodies/empty ones
                const message = body && body.message ? body.message : `Registration failed (${response.status})`;//it gives the error message with, if it does not exist then return the other thing
                throw new Error(message);
            }
            //setResult();
            //const data = await response.json();
        } catch (error) {
            setErrorResponse(error.message);
            console.log(error);
        }
    }

    return (
        <main>
            <h1>Registration</h1>
            <div className={styles.layout}>
                <section className={styles.editor}>
                    <form onSubmit={handleSubmit} className={styles.loginform}>
                        {/* <h1 className={styles.title}>Login</h1> */}
                        <label>Username</label>
                        <input
                            type="text"
                            id="userName"
                            name="userName"
                            // placeholder="Enter Username"
                            value={userName}
                            onChange={c => setUserName(c.target.value)}//refreshes the value inputted
                            maxLength="20"
                            required
                            className={styles.userinput} />
                        <label>Password</label>
                        <div className={styles.passwordWrapper}>
                            <input
                                type="password"
                                id="password"
                                name="password"
                                // placeholder="Enter Password"
                                value={password}
                                onChange={c => setPassword(c.target.value)}
                                maxLength="30"
                                className={styles.userinput}
                                required />
                            <button className={styles.showpassword} type="button" onClick={() => {
                                // document.getElementById("password").type = "text"
                                let passInput = document.getElementById("password");
                                let seePassImage = document.getElementById("image");
                                if (passInput.type === "password") {
                                    passInput.type = "text";
                                    seePassImage.src = "/src/assets/hide.png";
                                } else {
                                    passInput.type = "password";
                                    seePassImage.src = "/src/assets/show.png";
                                }
                            }}>
                                <img id="image" src="/src/assets/show.png" className={styles.image} /></button>
                        </div>
                        <label>Email</label>
                        <input
                            type="text"
                            id="email"
                            name="email"
                            // placeholder="Enter Email"
                            value={email}
                            onChange={c => setEmail(c.target.value)}
                            maxLength="30"
                            className={styles.userinput}
                            required />
                        {errorResponse && <p role="alert">{errorResponse}</p>}
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
        </main>
    )
}