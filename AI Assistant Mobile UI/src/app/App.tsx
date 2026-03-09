import { RouterProvider } from 'react-router';
import { router } from './routes';
import { AppToaster } from './components/ui/ErrorSnackbar';

export default function App() {
  return (
    <>
      <RouterProvider router={router} />
      <AppToaster />
    </>
  );
}
