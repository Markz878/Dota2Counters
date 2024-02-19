/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["./wwwroot/index.html", "./Pages/*.razor", "./Shared/*.razor", "./Components/*.razor"],
    theme: {
        extend: {
            animation: {
                fadeOutLeft: 'fadeOutLeftFrames 1s ease-in-out',
                fadeInRight: 'fadeInRightFrames 1s ease-in-out',
                fadeOutRight: 'fadeOutRightFrames 1s ease-in-out',
                fadeInLeft: 'fadeInLeftFrames 1s ease-in-out',
                wiggle: 'wiggle 1s ease-in-out infinite',
            },
            keyframes: {
                fadeOutLeftFrames: {
                    '0%': { translate: '0' },
                    '100%': { translate: '-100vw' },
                },
                fadeInRightFrames: {
                    '0%': { translate: '100vw' },
                    '100%': { translate: '0' },
                },
                fadeOutRightFrames: {
                    '0%': { translate: '0' },
                    '100%': { translate: '100vw' },
                },
                fadeInLeftFrames: {
                    '0%': { translate: '-100vw' },
                    '100%': { translate: '0' },
                },
                wiggle: {
                    '0%, 100%': {
                        transform: 'rotate(-3deg)'
                    },
                    '50%': {
                        transform: 'rotate(3deg)'
                    },
                }
            }
        }
    },
    plugins: [],
}
