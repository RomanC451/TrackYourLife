import { toast } from "sonner";


type ExerciseCreatedToastProps = {
    name: string;
}

const exerciseCreatedToast = ({ name }: ExerciseCreatedToastProps) => {
    toast.success(`${name}`, {
        description: `Exercise created successfully`,
    });
}

export default exerciseCreatedToast;
