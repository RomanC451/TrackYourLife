import http from "http";
import { URL } from "url";

const host = process.env.E2E_FOOD_API_HOST ?? "127.0.0.1";
const port = Number(process.env.E2E_FOOD_API_PORT ?? "9090");

function buildAuthResponse() {
  return {
    token_type: "Bearer",
    access_token: "e2e-access-token",
    expires_in: 3600,
    refresh_token: "e2e-refresh-token",
    user_id: "e2e-user",
  };
}

function buildFoodSearchResponse(query: string) {
  const foodId = Math.abs(hashCode(query)) % 1_000_000 + 1;

  return {
    items: [
      {
        item: {
          id: foodId,
          type: "food",
          brand_name: "E2E Foods",
          country_code: "US",
          description: `${query} (E2E)`,
          nutritional_contents: {
            energy: { value: 155, unit: "calories" },
            calcium: 50,
            carbohydrates: 1.1,
            cholesterol: 373,
            fat: 11,
            fiber: 0,
            iron: 1.8,
            monounsaturated_fat: 3.6,
            net_carbs: 1.1,
            polyunsaturated_fat: 1.4,
            potassium: 138,
            protein: 13,
            saturated_fat: 3.3,
            sodium: 124,
            sugar: 1.1,
            trans_fat: 0,
            vitamin_a: 520,
            vitamin_c: 0,
          },
          serving_sizes: [
            {
              id: foodId + 1,
              nutrition_multiplier: 1,
              unit: "Serving",
              value: 1,
            },
          ],
        },
      },
    ],
  };
}

function hashCode(value: string) {
  let hash = 0;
  for (let i = 0; i < value.length; i++) {
    hash = (hash << 5) - hash + value.charCodeAt(i);
    hash |= 0;
  }
  return hash;
}

const server = http.createServer((req, res) => {
  if (!req.url) {
    res.writeHead(400);
    res.end();
    return;
  }

  const url = new URL(req.url, `http://${host}:${port}`);

  if (url.pathname === "/health") {
    res.writeHead(200, { "Content-Type": "text/plain" });
    res.end("ok");
    return;
  }

  if (url.pathname === "/user/auth_token" && req.method === "GET") {
    res.writeHead(200, { "Content-Type": "application/json" });
    res.end(JSON.stringify(buildAuthResponse()));
    return;
  }

  if (url.pathname === "/v2/nutrition" && req.method === "GET") {
    const query = url.searchParams.get("q") ?? "food";
    res.writeHead(200, { "Content-Type": "application/json" });
    res.end(JSON.stringify(buildFoodSearchResponse(query)));
    return;
  }

  res.writeHead(404, { "Content-Type": "text/plain" });
  res.end("not found");
});

server.listen(port, host, () => {
  console.log(`E2E food API mock listening on http://${host}:${port}`);
});
