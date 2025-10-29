import type { Metadata } from 'next'
import { GeistSans } from 'geist/font/sans'
import { GeistMono } from 'geist/font/mono'
import { Analytics } from '@vercel/analytics/next'
import './globals.css'

import { Source_Serif_4 as V0_Font_Source_Serif_4 } from 'next/font/google'

// Initialize fonts
const _sourceSerif_4 = V0_Font_Source_Serif_4({
  subsets: ['latin'],
  weight: ["200","300","400","500","600","700","800","900"],
  variable: '--v0-font-source-serif-4',
})
const _v0_fontVariables = `${_sourceSerif_4.variable}`

export const metadata: Metadata = {
  title: 'Tower Destroyer',
  description: 'HTML5 game integrated with Beamable and Stellar',
  applicationName: 'Tower Destroyer',
  icons: {
    icon: '/favicon.ico',
  },
}

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode
}>) {
  return (
    <html lang="en">
      <body className={`font-sans ${GeistSans.variable} ${GeistMono.variable} ${_v0_fontVariables}`}>
        {children}
        <Analytics />
      </body>
    </html>
  )
}
