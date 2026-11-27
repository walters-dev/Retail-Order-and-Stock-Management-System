import styles from './Reports.module.css'
import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom';
export default function Report() {
    const [summary, setSummary] = useState({ count: 0, totalCompleted: 0, totalSales: 0, lowStock: [] });
    const [stock, setStock] = useState([]);
    const [errorResponse, setErrorResponse] = useState('');
    const token = localStorage.getItem('token');
    const [isLoading, setIsLoading] = useState(true);
    const [queryParameters, setQueryParameters] = useSearchParams();
    const currentPage = Number(queryParameters.get('page')) || 1;
    const pageSize = Number(queryParameters.get('Pagesize')) || 20;
    // useEffect(() => {
    // setQueryParameters((queryParameters)=>({
    //         'Pagesize':20
    //     }));
    // },[pageSize]);
    useEffect(() => {
        const controller = new AbortController()

        async function loadReports() {
            try {
                const response = await fetch('/api/reports/sales-summary', {
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": `Bearer ${token}`,
                    },
                });
                if (response.status == 403) {
                    throw new Error("You are not authorized to access reports");
                } else if (!response.ok) {
                    const body = await response.json().catch(() => null);//.catch(() => null) protects it from non json bodies/empty ones
                    const message = body && body.message ? body.message : `Reports request failed: (${response.status})`;
                    throw new Error(message);
                }
                //setResult();
                const data = await response.json();
                setSummary({
                    count: data.count,
                    totalCompleted: data.totalCompleted ?? 0,
                    totalSales: data.totalRevenue ?? 0,
                    orders: Array.isArray(data.orders) ? data.orders : [],
                });
            } catch (error) {
                setErrorResponse(error.message);
                console.log(error);
            }
            const params = new URLSearchParams({
                page: String(currentPage),
                Pagesize: String(pageSize),
            });
            try {
                const response = await fetch(`/api/reports/low-stock?${params}`, {
                    // method: "GET",
                    // signal: controller.signal,
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": `Bearer ${token}`,
                    },
                    //  query:{
                    //  Page: currentPage,
                    //  PageSize: 20,
                    //  }
                    //  body: JSON.stringify(data)//serializer
                });
                if (response.status == 403) {
                    throw new Error("You are not authorized to access reports");
                } else if (!response.ok) {
                    const body = await response.json().catch(() => null);//.catch(() => null) protects it from non json bodies/empty ones
                    const message = body && body.message ? body.message : `Reports request failed: (${response.status})`;
                    throw new Error(message);
                }
                //setResult();
                const result = await response.json();
                setStock(

                    Array.isArray(result) ? result : [],
                );
            } catch (error) {
                { errorResponse && <p role="alert">{errorResponse}</p> }
                console.log(error);
            }
            setIsLoading(false);
            // setResult(data.map((orders)=>({
            //     ...orders,
            //     id: orders.OrderId
            // })))

        }

        loadReports()

        return () => controller.abort()
    }, [currentPage])
    const handleNextPage = () => {
        setQueryParameters(prev => {
            const next = new URLSearchParams(prev);
            next.set('page', String(currentPage + 1));
            return next;
        });
    };

    const handlePreviousPage = () => {
        setQueryParameters(prev => {
            const next = new URLSearchParams(prev);
            next.set('page', String(Math.max(1, currentPage - 1)));
            return next;
        });
    };

    if (isLoading) {
        return <main><h1>Reports</h1><p>Loading...</p></main>;
    }
    return (
        <main>
            <h1>Reports</h1>
            <div className={styles.Layout}>
                <div className={styles.Summary}>
                    <div className={styles.CompletedOrders}>
                        <p>Completed Orders</p>
                        <h2>{summary.totalCompleted}</h2>
                    </div>
                    <div className={styles.TotalSales}>
                        <p>Total Sales </p>
                        <h2 className={styles.money}>R {summary.totalSales}</h2>
                    </div>
                </div>
                {errorResponse && <p role="alert">{errorResponse}</p>}
                <h3>Low-Stock Products</h3>
                <table>
                    <thead>
                        <tr>
                            <th scope="col">SKU</th>
                            <th scope="col">Name</th>
                            <th scope="col">Stock Quantity</th>
                        </tr>
                    </thead>
                    {stock.map((info) =>
                    (
                        <tbody key={info.productId}>
                            <tr >
                                <td>{info.sku}</td>
                                <td>{info.name}</td>
                                <td>{info.stockQuantity}</td>
                            </tr>
                        </tbody>
                    )
                    )}
                </table>
                <div className={styles.pageselection}>
                    <button onClick={handlePreviousPage} className={styles.buttons}>
                        Previous
                    </button>
                    <p>  Page {currentPage}</p>
                    <button onClick={handleNextPage} className={styles.buttons}>
                        Next
                    </button>
                </div>
            </div>
        </main>
    )
}