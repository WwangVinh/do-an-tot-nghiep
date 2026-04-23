import type { RouteObject } from 'react-router-dom'

import { RootLayout } from '../../layouts/RootLayout'
import { HomePage } from '../../pages/home/HomePage'
import { PricingPage } from '../../pages/pricing/PricingPage'
import { CarProductPage } from '../../pages/products/CarProductPage'
import { ProductsPage } from '../../pages/products/ProductsPage'
import { NewsPage } from '../../pages/news/NewsPage'
import { ContactPage } from '../../pages/contact/ContactPage'
import { InstallmentPage } from '../../pages/installment/InstallmentPage'
import { LaiThuPage } from '../../pages/lai-thu/LaiThuPage'
import { NotFoundPage } from '../../pages/NotFoundPage'
import { CreateCarFullPage } from '../../pages/admin/CreateCarFullPage'

export const routes: RouteObject[] = [
  {
    element: <RootLayout />,
    children: [
      { path: '/', element: <HomePage /> },
      { path: '/admin/cars/new', element: <CreateCarFullPage /> },
      { path: '/san-pham/xe/:carId', element: <CarProductPage /> },
      { path: '/san-pham', element: <ProductsPage /> },
      { path: '/bang-gia-xe', element: <PricingPage /> },
      { path: '/tin-tuc-uu-dai', element: <NewsPage /> },
      { path: '/tra-gop', element: <InstallmentPage /> },
      { path: '/lai-thu', element: <LaiThuPage /> },
      { path: '/lien-he', element: <ContactPage /> },
      { path: '*', element: <NotFoundPage /> },
    ],
  },
]

