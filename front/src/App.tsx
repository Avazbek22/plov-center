import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import PublicLayout from '@/components/layout/PublicLayout';
import Home from '@/pages/public/Home';
import Menu from '@/pages/public/Menu';
import Contacts from '@/pages/public/Contacts';
import Privacy from '@/pages/public/Privacy';
import Terms from '@/pages/public/Terms';
import Developers from '@/pages/public/Developers';
import NotFound from '@/pages/NotFound';
import Login from '@/pages/admin/Login';
import Dashboard from '@/pages/admin/Dashboard';
import Categories from '@/pages/admin/Categories';
import Dishes from '@/pages/admin/Dishes';
import Content from '@/pages/admin/Content';
import AdminLayout from '@/components/layout/AdminLayout';
import ProtectedRoute from '@/auth/ProtectedRoute';

const router = createBrowserRouter([
  {
    path: '/',
    element: <PublicLayout />,
    children: [
      { index: true, element: <Home /> },
      { path: 'menu', element: <Menu /> },
      { path: 'contacts', element: <Contacts /> },
      { path: 'privacy', element: <Privacy /> },
      { path: 'terms', element: <Terms /> },
      { path: 'developers', element: <Developers /> },
    ],
  },
  {
    path: '/admin/login',
    element: <Login />,
  },
  {
    path: '/admin',
    element: <ProtectedRoute />,
    children: [
      {
        element: <AdminLayout />,
        children: [
          {
            index: true,
            element: <Dashboard />,
          },
          {
            path: 'categories',
            element: <Categories />,
          },
          {
            path: 'dishes',
            element: <Dishes />,
          },
          {
            path: 'content',
            element: <Content />,
          },
        ],
      },
    ],
  },
  {
    path: '*',
    element: <NotFound />,
  },
]);

export default function App() {
  return <RouterProvider router={router} />;
}
