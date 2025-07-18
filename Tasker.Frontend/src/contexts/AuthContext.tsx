import {
  createContext,
  useState,
  useContext,
  type ReactNode,
  useEffect,
} from "react";
import { type UserLoginResponse } from "../types/User";
import { apiGet } from "../api/axios";

// Define the shape of our auth context
interface AuthContextType {
  user: UserLoginResponse | null;
  login: (userData: UserLoginResponse) => void;
  logout: () => void;
  isLoading: boolean;
}

// Create context with a meaningful default value
const AuthContext = createContext<AuthContextType>({
  user: null,
  login: () => console.warn("login function called without provider"),
  logout: () => console.warn("logout function called without provider"),
  isLoading: true,
});

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<UserLoginResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Load saved user on component mount
  useEffect(() => {
    const loadStoredUser = () => {
      try {
        const storedUserData = localStorage.getItem("userData");
        if (storedUserData) {
          const userData = JSON.parse(storedUserData);
          setUser(userData);
        }
      } catch (error) {
        clearAuthStorage();
        console.log(error);
      } finally {
        setIsLoading(false);
      }
    };

    loadStoredUser();
  }, []);

  // Check if token is valid on startup
  useEffect(() => {
    const validateToken = async () => {
      try {
        const res = await apiGet<UserLoginResponse>("Auth/account", true);

        if (!res.success) {
          logout();
          window.location.replace("/auth/login");
        } else {
          setUser(res.data);
        }
      } catch (err) {
        logout(); // fallback in case of unexpected error
        window.location.replace("/auth/login");
      }
    };

    validateToken();
  }, []);

  const login = (userData: UserLoginResponse) => {
    console.log("Login function called with:", userData);
    // Store auth data in localStorage
    localStorage.setItem("userData", JSON.stringify(userData));
    // Update state
    setUser(userData);
  };

  const logout = () => {
    console.log("Logout function called");
    clearAuthStorage();
    setUser(null);
  };

  // Create value object with auth state and functions
  const contextValue: AuthContextType = {
    user,
    login,
    logout,
    isLoading,
  };

  return (
    <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>
  );
};

// External user logout
export function clearAuthStorage() {
  localStorage.removeItem("userData");
}

// Custom hook for using the auth context
export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }

  return context;
};
