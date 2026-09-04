import { createContext, useCallback, useContext, useState, useEffect } from "react";
import * as api from "../api/auth";

const AuthContext = createContext();

const normalizeUser = (data) => data?.user ?? data ?? null;

const parseClaims = (claimsArray) => {
    if (!Array.isArray(claimsArray)) return claimsArray;
    const get = (type) => claimsArray.find((claim) => claim.type === type)?.value;
    return {
        id: get("sub") || get("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"),
        email: get("email") || get("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"),
        name: get("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"),
        role: get("http://schemas.microsoft.com/ws/2008/06/identity/claims/role"),
    };
};

const prepareUser = (rawData) => parseClaims(normalizeUser(rawData));

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(() => {
        const savedUser = localStorage.getItem("user");
        if (!savedUser) return null;
        try {
            return JSON.parse(savedUser);
        } catch {
            localStorage.removeItem("user");
            return null;
        }
    });

    const [isLoading, setIsLoading] = useState(true);

    const logout = useCallback(() => {
        localStorage.removeItem("authToken");
        localStorage.removeItem("user");
        setUser(null);
        setIsLoading(false);
    }, []);

    const validateSession = useCallback(async () => {
        try {
            const response = await api.getProfile(); 
            const userData = prepareUser(response.data);

            setUser(userData);
            localStorage.setItem("user", JSON.stringify(userData));

            setIsLoading(false);
        } catch (error) {
            console.error("Session validation failed", error);
            logout(); 
        }
    }, [logout]);

    const login = async (data) => {
        const response = await api.login(data);
        const payload = response.data;
        const token = payload.token;

        if (token) {
            localStorage.setItem("authToken", token);
            await validateSession();
        }
    };

    const register = async (data) => {
        const response = await api.register(data);
        const payload = response.data;
        const token = payload.token;

        if (token) {
            localStorage.setItem("authToken", token);
            await validateSession();
        }
    };

    useEffect(() => {
        const token = localStorage.getItem("authToken");
        if (!token) {
            setIsLoading(false);
            return;
        }
        validateSession();
    }, [validateSession]);

    return (
        <AuthContext.Provider value={{ user, isAuthenticated: !!user, isLoading, login, register, logout }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (context === undefined) throw new Error("useAuth must be used within an AuthProvider");
    return context;
};
