function OrderExercisesFormStepHeader() {
  return (
    <>
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-semibold">Step 2: Order Exercises</h2>
      </div>
      <p className="text-sm text-muted-foreground">
        Drag and drop exercises to set the order they'll appear in your workout.
      </p>
    </>
  );
}

export default OrderExercisesFormStepHeader;
