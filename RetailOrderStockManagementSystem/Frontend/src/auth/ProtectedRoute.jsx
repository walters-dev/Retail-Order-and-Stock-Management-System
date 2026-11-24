import { Navigate, Outlet, useLocation} from "react-router-dom";
import {useAuth} from "./authContext";

export default function ProtectedRoute(){
    const {role} = useAuth();
    const location = useLocation();

    if (role == null){
        return <Navigate to="/Login" state={{ from: location}} replace/>
    }
    return <Outlet/>;
}