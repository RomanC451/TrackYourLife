import { toast } from "sonner";

type ExerciseUpdatedToastProps = {
    name: string;
}

const exerciseUpdatedToast = ({ name }: ExerciseUpdatedToastProps) => {
    toast.success(`${name}`, {
        description: `Exercise updated successfully`,
    });
}

export default exerciseUpdatedToast;
