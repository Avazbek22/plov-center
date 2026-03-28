import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import PublicMenu from '@/pages/PublicMenu';
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
    element: <PublicMenu />,
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
