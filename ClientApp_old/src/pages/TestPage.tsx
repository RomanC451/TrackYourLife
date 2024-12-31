
type TestPageProps = {};

// export function useKeyboardHeight() {
//   const [keyboardHeight, setKeyboardHeight] = useState(0);

//   useEffect(() => {
//     const handleResize = () => {
//       if (!window.visualViewport) return;

//       const keyboardHeight = window.innerHeight - window.visualViewport.height;
//       setKeyboardHeight(keyboardHeight);
//     };

//     window.visualViewport?.addEventListener("resize", handleResize);
//     window.visualViewport?.addEventListener("scroll", handleResize);

//     return () => {
//       window.visualViewport?.removeEventListener("resize", handleResize);
//       window.visualViewport?.removeEventListener("scroll", handleResize);
//     };
//   }, []);

//   return keyboardHeight;
// }

function TestPage({}: TestPageProps): JSX.Element {
  // const keyboardHeight = useKeyboardHeight();

  return (
    // <div
    //   style={{
    //     transform: `translateY(-${keyboardHeight}px)`,
    //     transition: "transform 0.2s",
    //   }}
    //   className="absolute"
    // >
    //   {keyboardHeight}

    //   <Input />
    // </div>
    // <Test />
    <></>
  );
}

export default TestPage;
