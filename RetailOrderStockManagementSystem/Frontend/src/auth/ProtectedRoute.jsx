import { Navigate, Outlet, useLocation} from "react-router-dom";
import {useAuth} from "./authContext";

export function ProtectedRoute(){
    const {role} = useAuth();
    const location = useLocation();

    if ((role == null)|(!role)){
        return <Navigate to="/Login" state={{ from: location}} replace/>
    }
    return <Outlet/>;
}

export function ProtectedAdminStaffRoute(){
    const {role} = useAuth();
    const location = useLocation();

    if (role == "Manager"){
        return <Navigate to="/Login" state={{ from: location}} replace/>
    }
    return <Outlet/>;
}

export function ProtectedManagerAdminRoute(){
    const {role} = useAuth();
    const location = useLocation();

    if (role=="Staff"){
        return <Navigate to="/Login" state={{ from: location}} replace/>
    }
    return <Outlet/>;
}