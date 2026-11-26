import styles from './Reports.module.css'
import { useEffect, useState } from 'react'

export default function Report(){
    const[summary, setSummary] = useState({count: 0, totalCompleted: 0, totalSales: 0, lowStock: []});
    const[stock, setStock] = useState([]);
    const token = localStorage.getItem('token');
    useEffect(()=>{
        const controller = new AbortController()
    async function loadReports(){
        try {
            const response = await fetch('/api/reports/sales-summary', {
                // method: "GET",
                signal: controller.signal,
                headers: { 
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`,
                 },
              //  body: JSON.stringify(data)//serializer
            });
            if (!response.ok) {
                throw new Error('Reports request failed: ${response.status}');
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
            console.log(error);
        }
        try {
            const response = await fetch('/api/reports/low-stock', {
                // method: "GET",
                signal: controller.signal,
                headers: { 
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`,
                 },
              //  body: JSON.stringify(data)//serializer
            });
            if (!response.ok) {
                throw new Error('Reports request failed: ${response.status}');
            }
            //setResult();
            const result = await response.json();
            setStock(
                
                Array.isArray(result) ? result : [],
            );
        } catch (error) {
            console.log(error);
        }
        // setResult(data.map((orders)=>({
        //     ...orders,
        //     id: orders.OrderId
        // })))
        
}
loadReports()
return () => controller.abort()
}, [])
    return(
        <>
        <h1>Reports</h1>
        <div className ={styles.LowStockProducts}>
            <div className={styles.Summary}>
                <div className ={styles.CompletedOrders}>
                    <p>Completed Orders{summary.totalCompleted}</p>
                </div>
                <div className ={styles.TotalSales}>
                    <p>Total Sales {summary.totalSales} </p>
                </div>
            </div>
            <p>Low-Stock Products</p>
            <table>
                <th>sku</th>
                <th>name</th>
                <th>stock quantity</th>
            {stock.map((info)=>
                (
                    <tr key={info.productId}> 
                    <td>{info.sku}</td>
                    <td>{info.name}</td>
                    <td>{info.stockQuantity}</td>
                    </tr>
                )
            )}
            </table>
        </div>
        </>
    )
}