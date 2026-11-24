const salesSummary = async (data) => {
        try {
            const response = await fetch('/api/reports/sales-summary', {
                method: "GET",
                headers: { "Content-Type": "application/json" },//change cause of swagger??
                body: JSON.stringify(data)//serializer
            });
            if (!response.ok) {
                throw new Error('Reports request failed: ${response.status}');
            }
            const result = await response.json();
            if (result) {
                setToken(result.token);
                setRole(result.role);
                localStorage.setItem(TOKEN_KEY, token);
                localStorage.setItem(ROLE_KEY, JSON.stringify(role));
                router.navigate('/');//goes to index
            }
            throw new Error(result.message);
        } catch (error) {
            console.log(error);
        }
        
}


function Reports(){
    return(
        <>

        </>
    )
}